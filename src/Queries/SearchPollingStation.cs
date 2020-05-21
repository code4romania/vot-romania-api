using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class SearchPollingStation : IRequest<PagedResult<PollingStationModel>>
    {
        public PaginationQuery Pagination { get; }
        public PollingStationsQuery Query { get; }

        public SearchPollingStation(PaginationQuery pagination, PollingStationsQuery query)
        {
            Pagination = pagination;
            Query = query;
        }
    }
}