using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VotRomania.Models;
using VotRomania.Services.Location;
using VotRomania.Stores;
using VotRomania.Stores.Entities;

namespace VotRomania.Services
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IImportJobsRepository _importJobsRepository;
        private readonly IImportedPollingStationsRepository _importedPollingStationsRepository;
        private readonly IAddressLocationSearchService _locationSearchService;
        private readonly IBackgroundJobsQueue _jobsQueue;

        private readonly ILogger<QueuedHostedService> _logger;


        public QueuedHostedService(IBackgroundJobsQueue jobsQueue,
            IImportJobsRepository importJobsRepository,
            IImportedPollingStationsRepository importedPollingStationsRepository,
            IAddressLocationSearchService locationSearchService,
            ILogger<QueuedHostedService> logger)
        {
            _jobsQueue = jobsQueue;
            _importJobsRepository = importJobsRepository;
            _importedPollingStationsRepository = importedPollingStationsRepository;
            _locationSearchService = locationSearchService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var jobId = await _jobsQueue.DequeueAsync(stoppingToken);

                try
                {
                    var jobStatusResult = await _importJobsRepository.GetImportJobStatus(jobId);

                    if (jobStatusResult.IsFailure)
                    {
                        continue;
                    }

                    var jobStatus = jobStatusResult.Value.Status;
                    if (jobStatus == JobStatus.Canceled || jobStatus == JobStatus.Finished)
                    {
                        continue;
                    }

                    var query = new ImportedPollingStationsQuery
                    {
                        JobId = jobId,
                        ResolvedAddressStatus = ResolvedAddressStatusType.NotProcessed
                    };

                    var pollingStations = await _importedPollingStationsRepository.GetImportedPollingStationsAsync(query);

                    if (pollingStations.IsFailure)
                    {
                        continue;
                    }

                    if (pollingStations.Value.RowCount == 0)
                    {
                        await _importJobsRepository.UpdateJobStatus(jobId, JobStatus.Finished);
                        continue;
                    }

                    await _importJobsRepository.UpdateJobStatus(jobId, JobStatus.Started);

                    foreach (var ps in pollingStations.Value.Results)
                    {
                        var locationSearchResult = await _locationSearchService.FindCoordinates(ps.Address);

                        ps.ResolvedAddressStatus = locationSearchResult.OperationStatus;
                        ps.Latitude = locationSearchResult.Latitude;
                        ps.Longitude = locationSearchResult.Longitude;
                        ps.FailMessage = locationSearchResult.ErrorMessage;

                        await _importedPollingStationsRepository.UpdateImportedPollingStation(ps);
                    }

                    _jobsQueue.QueueBackgroundWorkItem(jobId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(jobId));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
