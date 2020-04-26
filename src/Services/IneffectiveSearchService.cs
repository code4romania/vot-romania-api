using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using VotRomania.Models;
using VotRomania.Providers;

namespace VotRomania.Services
{

    public class IneffectiveSearchService : IPollingStationSearchService
    {

        private class PollingStationGeoCoordinate
        {
            public GeoCoordinate Coordinates { get; set; }
            public PollingStationsInfo[] Data { get; set; }
        }

        private List<PollingStationGeoCoordinate> _pollingStations;


        public IneffectiveSearchService(IDataProvider dataProvider)
        {
            var pollingStations = dataProvider.LoadPollingStationsInfos();

            _pollingStations = pollingStations
                .GroupBy(x => new { lat = x.Lat, lng = x.Lng }, p => p, (key, pollingStationsInfos) => new PollingStationGeoCoordinate()
                {
                    Coordinates = new GeoCoordinate(key.lat, key.lng),
                    Data = pollingStationsInfos.ToArray()
                })
                .ToList();
        }

        public Task<PollingStationsInfo[]> GetNearestPollingStationsAsync(double latitude, double longitude)
        {
            var userLocation = new GeoCoordinate(latitude, longitude);
            var pollingStationsInfos = _pollingStations
                .OrderBy(x => x.Coordinates.GetDistanceTo(userLocation))
                .Take(6)
                .SelectMany(x => x.Data)
                .ToArray();

            return Task.FromResult(pollingStationsInfos);
        }
    }
}