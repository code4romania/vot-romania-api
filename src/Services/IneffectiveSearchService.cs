using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using VotRomania.Models;
using VotRomania.Stores;
using VotRomania.Stores.Entities;

namespace VotRomania.Services
{
    public class IneffectiveSearchService : IPollingStationSearchService
    {
        private readonly VotRomaniaContext _context;

        private List<(GeoCoordinate Coordinates, PollingStationModel[] PollingStations)> _pollingStations;

        // TODO: Change to KDTree
        public IneffectiveSearchService(VotRomaniaContext context)
        {
            _context = context;
            PrefillPollingStationsCache().GetAwaiter().GetResult();
        }

        private async Task PrefillPollingStationsCache()
        {
            var pollingStations = _context.PollingStations
                .Select(pollingStation => new PollingStationModel()
                {
                    Id = pollingStation.Id,
                    Address = pollingStation.Address,
                    Longitude = pollingStation.Longitude,
                    Latitude = pollingStation.Latitude,
                    County = pollingStation.County,
                    PollingStationNumber = pollingStation.PollingStationNumber,
                    Locality = pollingStation.Locality,
                    Institution = pollingStation.Institution,
                    AssignedAddresses = pollingStation.PollingStationAddresses.Select(a => MapToAssignedAddresses(a))
                })
                .ToListAsync();

            _pollingStations = pollingStations
                .GroupBy(
                    x => new
                    {
                        lat = x.Latitude,
                        lng = x.Longitude
                    },
                        p => p,
                        (key, pollingStationsInfos) => (new GeoCoordinate(key.lat, key.lng), pollingStationsInfos.ToArray())
                        )
                    .ToList();
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