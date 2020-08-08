using System;
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


        private List<(GeoCoordinate Coordinates, PollingStationModel[] PollingStations)>? _pollingStations;

        // TODO: Change to KDTree
        public IneffectiveSearchService(IPollingStationsRepository pollingStationsRepository)
        {
            _pollingStationsRepository = pollingStationsRepository;
            PrefillPollingStationsCache().GetAwaiter().GetResult();
        }

        private async Task PrefillPollingStationsCache()
        {
            var pollingStationsResult = await _pollingStationsRepository.GetPollingStationsAsync();

            if (pollingStationsResult.IsFailure)
            {
                throw new ApplicationException("could not load polling stations");
            }

            _pollingStations = pollingStationsResult
                .Value
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
                    PollingStations = x.PollingStations
                })
                .ToArray();

            return Task.FromResult(pollingStationsInfos);
        }

        public async Task BustCache()
        {
            await PrefillPollingStationsCache();
        }
    }
}