using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Extensions;
using VotRomania.Models;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
    public interface IImportedPollingStationsRepository
    {
        Task<Result> CleanPreviouslyImportedData();
        Task<Result> InsertPollingStations(Guid jobId, List<PollingStationModel> pollingStations);
        Task<Result> RemoveImportedPollingStations(Guid jobId);
        Task<Result<int>> GetNumberOfImportedAddresses(Guid jobId);
        Task<Result<int>> GetNumberOfUnresolvedAddresses(Guid jobId);
        Task<Result<PagedResult<ImportedPollingStationModel>>> GetImportedPollingStationsAsync(ImportedPollingStationsQuery? query = null, PaginationQuery? pagination = null);
        Task<Result> UpdateImportedPollingStation(ImportedPollingStationModel importedPollingStation);
    }

    public class ImportedPollingStationsRepository : IImportedPollingStationsRepository
    {
        private readonly VotRomaniaContext _context;
        private readonly ILogger<ImportedPollingStationsRepository> _logger;

        public ImportedPollingStationsRepository(VotRomaniaContext context, ILogger<ImportedPollingStationsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Result> CleanPreviouslyImportedData()
        {
            var result = await Result.Try(async () =>
            {
                _context.ImportedPollingStations.RemoveRange(_context.ImportedPollingStations);
                await _context.SaveChangesAsync();
            }, e => LogException(e));

            return result;
        }

        public async Task<Result> InsertPollingStations(Guid jobId, List<PollingStationModel> pollingStations)
        {
            var result = await Result.Try(async () =>
            {
                await _context.ImportedPollingStations.AddRangeAsync(pollingStations.Select(ps => MapToImportEntity(ps, jobId)));
                await _context.SaveChangesAsync();
            }, exception => LogException(exception));

            return result;
        }

        public async Task<Result> RemoveImportedPollingStations(Guid jobId)
        {
            var result = await Result.Try(async () =>
            {
                var importedPollingStationEntities = await _context.ImportedPollingStations
                    .Where(x => x.JobId == jobId.ToString())
                    .ToListAsync();

                _context.ImportedPollingStations.RemoveRange(importedPollingStationEntities);

                await _context.SaveChangesAsync();
            }, e => LogException(e));

            return result;
        }

        public async Task<Result<int>> GetNumberOfImportedAddresses(Guid jobId)
        {
            var result = await Result.Try(async () => await _context.ImportedPollingStations
                    .Where(x => x.JobId == jobId.ToString())
                    .CountAsync()
                , e => LogException(e));

            return result;
        }

        public async Task<Result<int>> GetNumberOfUnresolvedAddresses(Guid jobId)
        {
            var result = await Result.Try(async () => await _context.ImportedPollingStations
                    .Where(x => x.JobId == jobId.ToString())
                    .CountAsync(x => x.ResolvedAddressStatus != ResolvedAddressStatusType.Success)
                , e => LogException(e));

            return result;
        }

        public async Task<Result<PagedResult<ImportedPollingStationModel>>> GetImportedPollingStationsAsync(ImportedPollingStationsQuery? query = null, PaginationQuery? pagination = null)
        {
            var result = await Result.Try(async () =>
              {
                  var pollingStationsQuery = _context.ImportedPollingStations
                      .Select(pollingStation => new ImportedPollingStationModel
                      {
                          Id = pollingStation.Id,
                          Address = pollingStation.Address,
                          Longitude = pollingStation.Longitude,
                          Latitude = pollingStation.Latitude,
                          County = pollingStation.County,
                          PollingStationNumber = pollingStation.PollingStationNumber,
                          Locality = pollingStation.Locality,
                          Institution = pollingStation.Institution,
                          JobId = pollingStation.JobId,
                          ResolvedAddressStatus = pollingStation.ResolvedAddressStatus
                      });

                  if (query != null)
                  {
                      pollingStationsQuery = pollingStationsQuery
                          .Where(x => query.PollingStationId == null || x.Id == query.PollingStationId)
                          .Where(x => string.IsNullOrEmpty(query.County) || x.County.StartsWith(query.County))
                          .Where(x => string.IsNullOrEmpty(query.Locality) || x.Locality.StartsWith(query.Locality))
                          .Where(x => string.IsNullOrEmpty(query.Address) || x.Address.StartsWith(query.Address))
                          .Where(x => string.IsNullOrEmpty(query.PollingStationNumber) ||
                                      x.PollingStationNumber.StartsWith(query.PollingStationNumber))
                          .Where(x => string.IsNullOrEmpty(query.Institution) ||
                                      x.Institution.StartsWith(query.Institution))
                          .Where(x => query.JobId != null || x.JobId == query.JobId.ToString())
                          .Where(x => query.ResolvedAddressStatus == null ||
                                      x.ResolvedAddressStatus == query.ResolvedAddressStatus);
                  }

                  return await pollingStationsQuery.GetPaged(pagination?.PageNumber, pagination?.PageSize);

              });

            return result;
        }

        public async Task<Result> UpdateImportedPollingStation(ImportedPollingStationModel importedPollingStation)
        {
            var result = await Result.Try(async () =>
            {
                var pollingStation = await _context.ImportedPollingStations
                    .Where(x => x.Id == importedPollingStation.Id)
                    .FirstAsync();

                pollingStation.ResolvedAddressStatus = importedPollingStation.ResolvedAddressStatus;
                pollingStation.Latitude = importedPollingStation.Latitude;
                pollingStation.Longitude = importedPollingStation.Longitude;
                pollingStation.FailMessage = importedPollingStation.FailMessage;

                await _context.SaveChangesAsync();
            });

            return result;
        }


        private ImportedPollingStationEntity MapToImportEntity(PollingStationModel ps, Guid jobId)
        {
            var entity = new ImportedPollingStationEntity
            {
                Address = ps.Address,
                County = ps.County,
                Institution = ps.Institution,
                Locality = ps.Locality,
                PollingStationNumber = ps.PollingStationNumber,
                ResolvedAddressStatus = ResolvedAddressStatusType.NotProcessed,
                AssignedAddresses = new List<ImportedPollingStationAddressEntity>(),
                JobId = jobId.ToString()
            };

            if (ps.AssignedAddresses != null && ps.AssignedAddresses.Any())
            {
                foreach (var assignedAddress in ps.AssignedAddresses)
                {
                    entity.AssignedAddresses.Add(new ImportedPollingStationAddressEntity
                    {
                        HouseNumbers = assignedAddress.HouseNumbers,
                        Locality = assignedAddress.Locality,
                        Remarks = assignedAddress.Remarks,
                        Street = assignedAddress.Street,
                        StreetCode = assignedAddress.StreetCode
                    });
                }
            }

            return entity;
        }

        private string LogException(Exception exception, string? message = null)
        {
            var exceptionMessage = string.IsNullOrWhiteSpace(message) ? exception.Message : message;
            _logger.LogError(exception, exceptionMessage);
            return exceptionMessage;
        }
    }
}
