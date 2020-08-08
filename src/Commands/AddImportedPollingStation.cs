using System;
using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class AddImportedPollingStation : IRequest<Result<int>>
    {
        public Guid JobId { get; }
        public ImportedPollingStationUploadModel PollingStation { get; }

        public AddImportedPollingStation(Guid jobId, ImportedPollingStationUploadModel pollingStation)
        {
            JobId = jobId;
            PollingStation = pollingStation;
        }
    }
}