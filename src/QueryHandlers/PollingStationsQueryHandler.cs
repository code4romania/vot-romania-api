using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VotRomania.Models;
using VotRomania.Queries;
using VotRomania.Services;

namespace VotRomania.QueryHandlers
{
    public class PollingStationsQueryHandler : IRequestHandler<GetPollingStations, PollingStationsInfo[]>
    {
        private readonly IPollingStationSearchService _searchService;

        public PollingStationsQueryHandler(IPollingStationSearchService searchService)
        {
            _searchService = searchService;
        }
        public async Task<PollingStationsInfo[]> Handle(GetPollingStations request, CancellationToken cancellationToken)
        {
            var pollingStations = await _searchService.GetNearestPollingStationsAsync(request.Latitude, request.Longitude);

            return pollingStations;
        }
    }
}