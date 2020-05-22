using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VotRomania.Models;
using VotRomania.Queries;
using VotRomania.Services;
using VotRomania.Stores;

namespace VotRomania.QueryHandlers
{
    public class PollingStationsQueryHandler : IRequestHandler<GetNearbyPollingStations, PollingStationsGroupModel[]>,
        IRequestHandler<SearchPollingStation, PagedResult<PollingStationModel>>,
        IRequestHandler<GetPollingStationById, PollingStationModel>
    {
        private readonly IPollingStationSearchService _searchService;
        private readonly IPollingStationsRepository _repository;

        public PollingStationsQueryHandler(IPollingStationSearchService searchService, IPollingStationsRepository repository)
        {
            _searchService = searchService;
            _repository = repository;
        }
        public async Task<PollingStationsGroupModel[]> Handle(GetNearbyPollingStations request, CancellationToken cancellationToken)
        {
            var pollingStations = await _searchService.GetNearestPollingStationsAsync(request.Latitude, request.Longitude);

            return pollingStations;
        }

        public async Task<PagedResult<PollingStationModel>> Handle(SearchPollingStation request, CancellationToken cancellationToken)
        {
            var pollingStations = await _repository.GetPollingStationsAsync(request.Query, request.Pagination);

            return pollingStations;
        }

        public async Task<PollingStationModel> Handle(GetPollingStationById request, CancellationToken cancellationToken)
        {
            var pollingStation = await _repository.GetPollingStationAsync(request.PollingStationId);

            return pollingStation;
        }
    }
}