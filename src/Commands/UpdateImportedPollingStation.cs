using System;
using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class UpdateImportedPollingStation : IRequest<Result>
    {
        public Guid JobId { get; }
        public int PollingStationId { get; }
        public ImportedPollingStationUploadModel PollingStation { get; }

        public UpdateImportedPollingStation(Guid jobId, int pollingStationId, ImportedPollingStationUploadModel pollingStation)
        {
            JobId = jobId;
            PollingStationId = pollingStationId;
            PollingStation = pollingStation;
        }
    }
}