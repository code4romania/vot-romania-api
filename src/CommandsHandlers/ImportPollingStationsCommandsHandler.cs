using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Commands;
using VotRomania.Models;
using VotRomania.Services;
using VotRomania.Stores;
using VotRomania.Stores.Entities;

namespace VotRomania.CommandsHandlers
{
    public class ImportPollingStationsCommandsHandler :
        IRequestHandler<StartImportNewPollingStations, Result<Guid>>,
        IRequestHandler<CancelImportJob, Result>,
        IRequestHandler<CompleteImportJob, Result>,
        IRequestHandler<RestartImportJob, Result>,
        IRequestHandler<GetImportJobStatus, Result<JobStatusModel>>,
        IRequestHandler<GetCurrentImportJob, Result<JobStatusModel>>

    {
        private readonly VotRomaniaContext _context;
        private readonly IImportJobsRepository _importJobsRepository;
        private readonly IPollingStationsRepository _pollingStationsRepository;
        private readonly IImportedPollingStationsRepository _importedPollingStationsRepository;
        private readonly ILogger<ImportPollingStationsCommandsHandler> _logger;
        private readonly IBackgroundJobsQueue _backgroundJobsQueue;
        private readonly IPollingStationSearchService _pollingStationSearchService;
        private readonly IExcelParser _excelParser;

        public ImportPollingStationsCommandsHandler(
            VotRomaniaContext context,
            IImportJobsRepository importJobsRepository,
            IPollingStationsRepository pollingStationsRepository,
            IImportedPollingStationsRepository importedPollingStationsRepository,
            IBackgroundJobsQueue backgroundJobsQueue,
            IPollingStationSearchService pollingStationSearchService,
            IExcelParser excelParser,
            ILogger<ImportPollingStationsCommandsHandler> logger)
        {
            _context = context;
            _importJobsRepository = importJobsRepository;
            _pollingStationsRepository = pollingStationsRepository;
            _importedPollingStationsRepository = importedPollingStationsRepository;
            _backgroundJobsQueue = backgroundJobsQueue;
            _pollingStationSearchService = pollingStationSearchService;
            _excelParser = excelParser;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(StartImportNewPollingStations request, CancellationToken cancellationToken)
        {

            Guid jobId = Guid.NewGuid();
            return await _importJobsRepository.HasImportJobInProgress()
                .Ensure(result => result == false, "Cannot start an upload job while an upload is in progress")
                .Tap(() => _importedPollingStationsRepository.CleanPreviouslyImportedData())
                .Bind(_ => _excelParser.ParsePollingStations(request.File))
                .Tap(ps => _importedPollingStationsRepository.InsertPollingStations(jobId, ps))
                .Tap(() => _importJobsRepository.InsertInJobTable(jobId, request.File))
                .Bind(_ => _backgroundJobsQueue.QueueBackgroundWorkItem(jobId))
                .Finally(r => r.IsSuccess ? Result.Success(jobId) : Result.Failure<Guid>(r.Error));
        }

        private string LogException(Exception exception, string? message = null)
        {
            var exceptionMessage = string.IsNullOrWhiteSpace(message) ? exception.Message : message;
            _logger.LogError(exception, exceptionMessage);
            return exceptionMessage;
        }

        private static PollingStationEntity MapToPollingStationEntity(ImportedPollingStationEntity ips)
        {
            var entity = new PollingStationEntity
            {
                Address = ips.Address,
                County = ips.County,
                Institution = ips.Institution,
                Locality = ips.Locality,
                PollingStationNumber = ips.PollingStationNumber,
                Latitude = ips.Latitude ?? -999,
                Longitude = ips.Longitude ?? -999,
                PollingStationAddresses = new List<PollingStationAddressEntity>()
            };

            if (ips.AssignedAddresses != null && ips.AssignedAddresses.Any())
            {
                foreach (var assignedAddress in ips.AssignedAddresses)
                {
                    entity.PollingStationAddresses.Add(new PollingStationAddressEntity
                    {
                        Locality = assignedAddress.Locality,
                        HouseNumbers = assignedAddress.HouseNumbers,
                        Remarks = assignedAddress.Remarks,
                        Street = assignedAddress.Street,
                        StreetCode = assignedAddress.StreetCode
                    });
                }
            }

            return entity;
        }

        public async Task<Result> Handle(CancelImportJob request, CancellationToken cancellationToken)
        {
            return await _importJobsRepository.CancelImportJob(request.JobId);
        }

        public async Task<Result<JobStatusModel>> Handle(GetImportJobStatus request, CancellationToken cancellationToken)
        {
            return await _importJobsRepository.GetImportJobStatus(request.JobId);
        }
        public async Task<Result> Handle(CompleteImportJob request, CancellationToken cancellationToken)
        {
            return await _importJobsRepository.GetImportJobStatus(request.JobId)
                .Ensure(job => job.Status == JobStatus.Finished,
                    "Cannot complete job. Job should have finished status.")
                .Bind(_ => _importedPollingStationsRepository.GetNumberOfImportedAddresses(request.JobId))
                .Ensure(count => count > 0, "There are no polling stations to import.")
                .Bind(_ => _importedPollingStationsRepository.GetNumberOfUnresolvedAddresses(request.JobId))
                .Ensure(numberOfUnresolvedAddresses => numberOfUnresolvedAddresses == 0,
                    "Cannot complete job with unresolved addresses.")
                .Tap(() => _pollingStationsRepository.RemoveAllPollingStations())
                .Tap(() => AddImportedPollingStationsToPollingStations(request.JobId, cancellationToken))
                .Tap(() => _importedPollingStationsRepository.RemoveImportedPollingStations(request.JobId))
                .Tap(() => UpdateAddressBank())
                .Tap(() => _importJobsRepository.UpdateJobStatus(request.JobId, JobStatus.Imported))
                .Tap(() => _pollingStationSearchService.BustCache());
        }

        private async Task UpdateAddressBank()
        {
            _ = await _context.Database.ExecuteSqlRawAsync("call PopulateAddressBank()");
        }

        // TODO: if we will use different db move this operation to a stored procedure
        private async Task<Result> AddImportedPollingStationsToPollingStations(Guid jobId, CancellationToken cancellationToken)
        {
            var result = await Result.Try(async () =>
            {
                var pollingStationEntities = await _context.ImportedPollingStations
                    .Include(x => x.AssignedAddresses)
                    .Where(x => x.JobId == jobId.ToString())
                    .Select(x => MapToPollingStationEntity(x))
                    .ToListAsync(cancellationToken);


                foreach (var entity in pollingStationEntities)
                {
                    await _context.PollingStations.AddAsync(entity, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }, e => LogException(e));
            return result;
        }

        public async Task<Result<JobStatusModel>> Handle(GetCurrentImportJob request, CancellationToken cancellationToken)
        {
            return await _importJobsRepository.GetCurrentImportJob();
        }

        public async Task<Result> Handle(RestartImportJob request, CancellationToken cancellationToken)
        {
            return await _importJobsRepository.RestartJob(request.JobId)
                .Tap(() => _backgroundJobsQueue.QueueBackgroundWorkItem(request.JobId));
        }
    }
}
