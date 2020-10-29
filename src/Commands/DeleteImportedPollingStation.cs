using System;
using CSharpFunctionalExtensions;
using MediatR;

namespace VotRomania.Commands
{
    public class DeleteImportedPollingStation : IRequest<Result>
    {
        public Guid JobId { get; }
        public int PollingStationId { get; }

        public DeleteImportedPollingStation(Guid jobId, int pollingStationId)
        {
            JobId = jobId;
            PollingStationId = pollingStationId;
        }
    }
}