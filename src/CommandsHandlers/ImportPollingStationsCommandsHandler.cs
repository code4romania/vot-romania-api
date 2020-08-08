using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        public ImportPollingStationsCommandsHandler(
            VotRomaniaContext context,
            IImportJobsRepository importJobsRepository,
            IPollingStationsRepository pollingStationsRepository,
            IImportedPollingStationsRepository importedPollingStationsRepository,
            IBackgroundJobsQueue backgroundJobsQueue,
            IPollingStationSearchService pollingStationSearchService,
            ILogger<ImportPollingStationsCommandsHandler> logger)
        {
            _context = context;
            _importJobsRepository = importJobsRepository;
            _pollingStationsRepository = pollingStationsRepository;
            _importedPollingStationsRepository = importedPollingStationsRepository;
            _backgroundJobsQueue = backgroundJobsQueue;
            _pollingStationSearchService = pollingStationSearchService;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(StartImportNewPollingStations request, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                Guid jobId = Guid.NewGuid();
                return await _importJobsRepository.HasImportJobInProgress()
                    .Ensure(result => result == false, "Cannot start an upload job while an upload is in progress")
                    .Tap(() => _importedPollingStationsRepository.CleanPreviouslyImportedData())
                    .Bind(_ => ParsePollingStations(request.File))
                    .Tap(ps => _importedPollingStationsRepository.InsertPollingStations(jobId, ps))
                    .Tap(() => _importJobsRepository.InsertInJobTable(jobId, request.File))
                    .Bind(_ => _backgroundJobsQueue.QueueBackgroundWorkItem(jobId))
                    .OnFailure(() => transaction.RollbackAsync(cancellationToken))
                    .Bind(async () => await Result.Try(async () => await transaction.CommitAsync(cancellationToken)))
                    .Finally(r => r.IsSuccess ? Result.Success(jobId) : Result.Failure<Guid>(r.Error));
            }
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
                        HouseNumbers = assignedAddress.HouseNumbers,
                        Locality = assignedAddress.Locality,
                        Remarks = assignedAddress.Remarks,
                        Street = assignedAddress.Street,
                        StreetCode = assignedAddress.StreetCode
                    });
                }
            }

            return entity;
        }

        private Result<List<PollingStationModel>> ParsePollingStations(IFormFile requestFile)
        {
            var parseResult = Result.Try(() =>
            {
                DataSet result;

                using (var reader = ExcelReaderFactory.CreateReader(requestFile.OpenReadStream()))
                {
                    result = reader.AsDataSet();
                }

                var pollingStationsData = result.Tables[0];
                var pollingStations = new List<PollingStationModel>();

                PollingStationModel? currentPollingStation = null;
                int index = 1;
                do
                {
                    DataRow row = pollingStationsData.Rows[index];

                    var pollingStationNumber = GetString(row[4]);
                    if (string.IsNullOrEmpty(pollingStationNumber))
                    {
                        do
                        {
                            AssignedAddresses assignedAddress = new AssignedAddresses
                            {
                                HouseNumbers = GetString(pollingStationsData.Rows[index][12]),
                                Locality = GetString(pollingStationsData.Rows[index][9]),
                                Remarks = GetString(pollingStationsData.Rows[index][13]),
                                Street = GetString(pollingStationsData.Rows[index][11]),
                                StreetCode = GetString(pollingStationsData.Rows[index][10]),

                            };
                            currentPollingStation?.AssignedAddresses.Add(assignedAddress);
                            ++index;

                        } while (index < pollingStationsData.Rows.Count &&
                                 string.IsNullOrEmpty(GetString(pollingStationsData.Rows[index][4])));

                        --index;
                    }
                    else
                    {
                        currentPollingStation = new PollingStationModel
                        {
                            County = GetString(row[0]),
                            PollingStationNumber = pollingStationNumber,
                            Locality = GetString(row[9]),
                            Institution = GetString(row[6]),
                            Address = GetString(row[7]),
                            AssignedAddresses = new List<AssignedAddresses>()
                        };

                        pollingStations.Add(currentPollingStation);
                    }

                    ++index;

                } while (index < pollingStationsData.Rows.Count);

                return pollingStations;
            }, exception => LogException(exception, $"Error in method {nameof(ParsePollingStations)}"));

            return parseResult;
        }

        private static string GetString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var text = value.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return text.Trim();
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
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                return await _importJobsRepository.GetImportJobStatus(request.JobId)
                    .Ensure(job => job.Status == JobStatus.Finished, "Cannot complete job. Job should have finished status.")
                    .Bind(_ => _importedPollingStationsRepository.GetNumberOfImportedAddresses(request.JobId))
                    .Ensure(count => count > 0, "There are no polling stations to import.")
                    .Bind(_ => _importedPollingStationsRepository.GetNumberOfUnresolvedAddresses(request.JobId))
                    .Ensure(numberOfUnresolvedAddresses => numberOfUnresolvedAddresses == 0,
                        "Cannot complete job with unresolved addresses.")
                    .Tap(() => _pollingStationsRepository.RemoveAllPollingStations())
                    .Tap(() => AddImportedPollingStationsToPollingStations(request.JobId, cancellationToken))
                    .Tap(() => _importedPollingStationsRepository.RemoveImportedPollingStations(request.JobId))
                    .Tap(() => _importJobsRepository.UpdateJobStatus(request.JobId, JobStatus.Imported))
                    .Tap(() => _pollingStationSearchService.BustCache())
                    .OnFailure(() => transaction.RollbackAsync(cancellationToken))
                    .OnSuccessTry(_ => transaction.CommitAsync(cancellationToken));
            }
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
    }
}
