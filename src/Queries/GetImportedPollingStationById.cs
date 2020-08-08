using System;
using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class GetImportedPollingStationById : IRequest<Result<ImportedPollingStationModel>>
    {
        public Guid JobId { get; }
        public int PollingStationId { get; }

        public GetImportedPollingStationById(Guid jobId, int pollingStationId)
        {
            JobId = jobId;
            PollingStationId = pollingStationId;
        }
    }
}