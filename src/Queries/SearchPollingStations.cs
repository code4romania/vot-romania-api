using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class SearchPollingStations : IRequest<Result<PagedResult<PollingStationModel>>>
    {
        public PaginationQuery Pagination { get; }
        public PollingStationsQuery Query { get; }

        public SearchPollingStations(PaginationQuery pagination, PollingStationsQuery query)
        {
            Pagination = pagination;
            Query = query;
        }
    }
}