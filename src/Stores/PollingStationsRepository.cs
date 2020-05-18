using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Models;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
    public interface IPollingStationsRepository
    {
        Task<List<PollingStationModel>> GetPollingStationsAsync();
        Task<bool> AddPollingStationAsync(PollingStationModel pollingStation);
    }
    public class PollingStationsRepository : IPollingStationsRepository
    {
        private readonly ILogger<PollingStationsRepository> _logger;
        private readonly VotRomaniaContext _context;

        public PollingStationsRepository(
            ILogger<PollingStationsRepository> logger,
            VotRomaniaContext context)
        {
            _logger = logger;
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task<List<PollingStationModel>> GetPollingStationsAsync()
        {
            return await _context.PollingStations
                .Select(pollingStation => new PollingStationModel()
                {
                    Id = pollingStation.Id,
                    Address = pollingStation.Address,
                    Longitude = pollingStation.Longitude,
                    Latitude = pollingStation.Latitude,
                    County = pollingStation.County,
                    PollingStationNumber = pollingStation.PollingStationNumber,
                    Locality = pollingStation.Locality,
                    Institution = pollingStation.Institution
                })
                .ToListAsync();
        }

        public Task<bool> AddPollingStationAsync(PollingStationModel pollingStation)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var entity = new PollingStationEntity
                {
                    Id = pollingStation.Id,
                    Address = pollingStation.Address,
                    Longitude = pollingStation.Longitude,
                    Latitude = pollingStation.Latitude,
                    County = pollingStation.County,
                    PollingStationNumber = pollingStation.PollingStationNumber,
                    Locality = pollingStation.Locality,
                    Institution = pollingStation.Institution
                };

                _context.PollingStations.Add(entity);
                _context.SaveChanges();

                transaction.Commit();
            }
            return Task.FromResult(true);
        }
    }
}
