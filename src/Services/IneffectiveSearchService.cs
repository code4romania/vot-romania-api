using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using VotRomania.Models;
using VotRomania.Stores;

namespace VotRomania.Services
{
    public class IneffectiveSearchService : IPollingStationSearchService
    {
        private readonly IPollingStationsRepository _pollingStationsRepository;


        private readonly List<(GeoCoordinate Coordinates, PollingStationModel[] PollingStations)> _pollingStations;

        // TODO: Change to KDTree
        public IneffectiveSearchService(IPollingStationsRepository pollingStationsRepository)
        {
            _pollingStationsRepository = pollingStationsRepository;
            var pollingStations = pollingStationsRepository.GetPollingStationsAsync().GetAwaiter().GetResult();

            _pollingStations = pollingStations
                .Results
                .GroupBy(
                    x => new { lat = x.Latitude, lng = x.Longitude },
                    p => p,
                    (key, pollingStationsInfos) => (new GeoCoordinate(key.lat, key.lng), pollingStationsInfos.ToArray())
                    )
                .ToList();
        }

        public Task<PollingStationsGroupModel[]> GetNearestPollingStationsAsync(double latitude, double longitude)
        {
            var userLocation = new GeoCoordinate(latitude, longitude);
            var pollingStationsInfos = _pollingStations
                .OrderBy(x => x.Coordinates.GetDistanceTo(userLocation))
                .Take(6)
                .Select(x => new PollingStationsGroupModel()
                {
                    Latitude = x.Coordinates.Latitude,
                    Longitude = x.Coordinates.Longitude,
                    PollingStations = x.PollingStations,
                    Distance = x.Coordinates.GetDistanceTo(userLocation)
                })
                .ToArray();

            return Task.FromResult(pollingStationsInfos);
        }
    }
}