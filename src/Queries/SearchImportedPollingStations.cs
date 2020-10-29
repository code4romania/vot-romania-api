using System;
using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class SearchImportedPollingStations : IRequest<Result<PagedResult<ImportedPollingStationModel>>>
    {
        public Guid JobId { get; }
        public PaginationQuery Pagination { get; }
        public ImportedPollingStationsQuery Query { get; }

        public SearchImportedPollingStations(Guid jobId, PaginationQuery pagination, ImportedPollingStationsQuery query)
        {
            JobId = jobId;
            Pagination = pagination;
            Query = query;
        }
    }
}