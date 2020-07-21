using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Models;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
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

        public async Task<PagedResult<PollingStationModel>> GetPollingStationsAsync(PollingStationsQuery query = null, PaginationQuery pagination = null)
        {
            var pollingStationsQuery = _context.PollingStations
                .Include(x => x.PollingStationAddresses)
                .Select(pollingStation => new PollingStationModel
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
                });

            if (query != null)
            {
                pollingStationsQuery = pollingStationsQuery.Where(x => query.PollingStationId == null || x.Id == query.PollingStationId)
                    .Where(x => string.IsNullOrEmpty(query.County) || x.County.StartsWith(query.County))
                    .Where(x => string.IsNullOrEmpty(query.Locality) || x.Locality.StartsWith(query.Locality))
                    .Where(x => string.IsNullOrEmpty(query.Address) || x.Address.StartsWith(query.Address))
                    .Where(x => string.IsNullOrEmpty(query.PollingStationNumber) || x.PollingStationNumber.StartsWith(query.PollingStationNumber))
                    .Where(x => string.IsNullOrEmpty(query.Institution) || x.Institution.StartsWith(query.Institution));
            }


            return await pollingStationsQuery.GetPaged(pagination?.PageNumber, pagination?.PageSize);

        }

        public async Task<PollingStationModel> GetPollingStationAsync(int pollingStationId)
        {
            var pollingStationsQuery = _context.PollingStations
                .Include(x => x.PollingStationAddresses)
                .Where(x => x.Id == pollingStationId)
                .Select(pollingStation => new PollingStationModel
                {
                    Id = pollingStation.Id,
                    Address = pollingStation.Address,
                    Longitude = pollingStation.Longitude,
                    Latitude = pollingStation.Latitude,
                    County = pollingStation.County,
                    PollingStationNumber = pollingStation.PollingStationNumber,
                    Locality = pollingStation.Locality,
                    Institution = pollingStation.Institution,
                    AssignedAddresses = pollingStation.PollingStationAddresses.Select(x => MapToAssignedAddresses(x))
                });

            return await pollingStationsQuery.SingleOrDefaultAsync();
        }

        private static AssignedAddresses MapToAssignedAddresses(PollingStationAddressEntity pollingStationAssignedAddress)
        {
            if (pollingStationAssignedAddress == null)
            {
                return null;
            }

            return new AssignedAddresses
            {
                Id = pollingStationAssignedAddress.Id,
                PollingStationId = pollingStationAssignedAddress.PollingStationId,
                HouseNumbers = pollingStationAssignedAddress.HouseNumbers,
                Remarks = pollingStationAssignedAddress.Remarks,
                Street = pollingStationAssignedAddress.Street,
                Locality = pollingStationAssignedAddress.Locality,
                StreetCode = pollingStationAssignedAddress.StreetCode
            };
        }

        public async Task<(bool isSuccess, string errorMessage, int pollingStationId)> AddPollingStationAsync(PollingStationModel pollingStation)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var newId = await _context.PollingStations.MaxAsync(x => x.Id) + 1;
                    var entity = new PollingStationEntity
                    {
                        Id = newId,
                        Address = pollingStation.Address,
                        Longitude = pollingStation.Longitude,
                        Latitude = pollingStation.Latitude,
                        County = pollingStation.County,
                        PollingStationNumber = pollingStation.PollingStationNumber,
                        Locality = pollingStation.Locality,
                        Institution = pollingStation.Institution,
                    };

                    await _context.PollingStations.AddAsync(entity);
                    _context.SaveChanges();

                    transaction.Commit();
                    return (true, string.Empty, newId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add new polling station");
                return (false, e.Message, -1);
            }
        }
        public async Task<(bool isSuccess, string errorMessage)> DeletePollingStationAsync(int pollingStationId)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var entity = await _context.PollingStations.FirstOrDefaultAsync(x => x.Id == pollingStationId);
                    if (entity == null)
                    {
                        return (false, $"Could not find polling station with id = {pollingStationId}");
                    }

                    _context.PollingStations.Remove(entity);
                    _context.SaveChanges();

                    transaction.Commit();
                    return (true, string.Empty);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not delete pollingStation with id ={pollingStationId}");
                return (false, e.Message);
            }
        }

        public async Task<(bool isSuccess, string errorMessage)> UpdatePollingStationAsync(PollingStationModel pollingStation)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var entity = await _context.PollingStations
                        .FirstOrDefaultAsync(x => x.Id == pollingStation.Id);

                    if (entity == null)
                    {
                        return (false, $"Could not find polling station with id = {pollingStation.Id}");
                    }

                    entity.Address = pollingStation.Address;
                    entity.Longitude = pollingStation.Longitude;
                    entity.Latitude = pollingStation.Latitude;
                    entity.County = pollingStation.County;
                    entity.PollingStationNumber = pollingStation.PollingStationNumber;
                    entity.Locality = pollingStation.Locality;
                    entity.Institution = pollingStation.Institution;

                    _context.SaveChanges();

                    transaction.Commit();
                    return (true, string.Empty);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not update entity with id = {pollingStation.Id}");
                return (false, e.Message);
            }
        }
    }
}
