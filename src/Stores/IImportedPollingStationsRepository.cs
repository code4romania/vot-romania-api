using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Extensions;
using VotRomania.Models;
using VotRomania.Services.Location;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
    public interface IImportedPollingStationsRepository
    {
        Task<Result> CleanPreviouslyImportedData();
        Task<Result> InsertPollingStations(Guid jobId, List<PollingStationModel> pollingStations);
        Task<Result<int>> AddPollingStation(Guid jobId, ImportedPollingStationModel pollingStation);
        Task<Result> RemoveImportedPollingStations(Guid jobId);
        Task<Result<int>> GetNumberOfImportedAddresses(Guid jobId);
        Task<Result<int>> GetNumberOfUnresolvedAddresses(Guid jobId);
        Task<Result<PagedResult<ImportedPollingStationModel>>> GetImportedPollingStationsAsync(Guid jobId,
            ImportedPollingStationsQuery? query = null, PaginationQuery? pagination = null);
        Task<Result> UpdateImportedPollingStation(Guid requestJobId, ImportedPollingStationModel importedPollingStation);
        Task<Result> DeleteImportedPollingStation(Guid jobId, int pollingStationId);
        Task<Result<ImportedPollingStationModel>> GetImportedPollingStationById(Guid jobId, int importedPollingStationId);
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

        public async Task<Result<int>> AddPollingStation(Guid jobId, ImportedPollingStationModel pollingStation)
        {
            var result = await Result.Try(async () =>
            {
                var entity = new ImportedPollingStationEntity
                {
                    Address = pollingStation.Address,
                    County = pollingStation.County,
                    Institution = pollingStation.Institution,
                    Locality = pollingStation.Locality,
                    PollingStationNumber = pollingStation.PollingStationNumber,
                    ResolvedAddressStatus = pollingStation.ResolvedAddressStatus,
                    FailMessage = pollingStation.FailMessage,
                    Latitude = pollingStation.Latitude,
                    Longitude = pollingStation.Longitude,
                    JobId = jobId.ToString(),
                };
                entity.AssignedAddresses = pollingStation.AssignedAddresses
                    .Select(x => MapToAssignedAddresses(entity, x)).ToList();

                await _context.ImportedPollingStations.AddAsync(entity);
                await _context.SaveChangesAsync();

                return entity.Id;
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

        public async Task<Result<PagedResult<ImportedPollingStationModel>>> GetImportedPollingStationsAsync(Guid jobId, ImportedPollingStationsQuery? query = null, PaginationQuery? pagination = null)
        {
            var result = await Result.Try(async () =>
              {
                  var pollingStationsQuery = _context.ImportedPollingStations
                      .Where(x => x.JobId == jobId.ToString())
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
                          ResolvedAddressStatus = pollingStation.ResolvedAddressStatus,
                          FailMessage = pollingStation.FailMessage,
                          AssignedAddresses = pollingStation.AssignedAddresses.Select(x => new AssignedAddressModel()
                          {
                              HouseNumbers = x.HouseNumbers,
                              Locality = x.Locality,
                              Remarks = x.Remarks,
                              Street = x.Street,
                              PollingStationId = x.ImportedPollingStationId,
                              StreetCode = x.StreetCode,
                              Id = x.Id
                          }).ToArray()
                      });

                  if (query != null)
                  {
                      pollingStationsQuery = pollingStationsQuery
                          .ConditionalWhere(() => !string.IsNullOrWhiteSpace(query.County), q => EF.Functions.Like(q.County, AddStartsWithPattern(query.County)))
                              .ConditionalWhere(() => !string.IsNullOrWhiteSpace(query.Locality), q => EF.Functions.Like(q.Locality, AddStartsWithPattern(query.Locality)))
                              .ConditionalWhere(() => !string.IsNullOrWhiteSpace(query.Address), q => EF.Functions.Like(q.Address, AddStartsWithPattern(query.Address)))
                              .ConditionalWhere(() => !string.IsNullOrWhiteSpace(query.PollingStationNumber), q => EF.Functions.Like(q.PollingStationNumber, AddStartsWithPattern(query.PollingStationNumber)))
                              .ConditionalWhere(() => !string.IsNullOrWhiteSpace(query.Institution), q => EF.Functions.Like(q.Institution, AddStartsWithPattern(query.Institution)))
                              .ConditionalWhere(() => query.ResolvedAddressStatus != null, q => q.ResolvedAddressStatus == query.ResolvedAddressStatus);
                  }

                  return await pollingStationsQuery.GetPaged(pagination?.PageNumber, pagination?.PageSize);

              });

            return result;
        }

        private static string AddStartsWithPattern(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return $"{value}%";
        }

        public async Task<Result> UpdateImportedPollingStation(Guid jobId, ImportedPollingStationModel importedPollingStation)
        {
            var result = await Result.Try(async () =>
            {
                var pollingStation = await _context.ImportedPollingStations
                    .Where(x => x.Id == importedPollingStation.Id)
                    .Where(x => x.JobId == jobId.ToString())
                    .FirstAsync();

                pollingStation.County = importedPollingStation.County;
                pollingStation.Locality = importedPollingStation.Locality;
                pollingStation.PollingStationNumber = importedPollingStation.PollingStationNumber;
                pollingStation.Address = importedPollingStation.Address;
                pollingStation.Institution = importedPollingStation.Institution;
                pollingStation.Latitude = importedPollingStation.Latitude;
                pollingStation.Longitude = importedPollingStation.Longitude;
                pollingStation.ResolvedAddressStatus = importedPollingStation.ResolvedAddressStatus;
                pollingStation.FailMessage = importedPollingStation.FailMessage;
                pollingStation.JobId = jobId.ToString();

                if (importedPollingStation.AssignedAddresses != null)
                {
                    if (pollingStation.AssignedAddresses != null)
                    {
                        _context.ImportedPollingStationAddresses.RemoveRange(pollingStation.AssignedAddresses);
                    }

                    pollingStation.AssignedAddresses = importedPollingStation.AssignedAddresses.Select(x => MapToAssignedAddresses(pollingStation, x)).ToList();
                }

                await _context.SaveChangesAsync();
            });

            return result;
        }

        private static ImportedPollingStationAddressEntity MapToAssignedAddresses(ImportedPollingStationEntity pollingStation, AssignedAddressModel assignedAddress)
        {
            return new ImportedPollingStationAddressEntity
            {
                Id = assignedAddress.Id,
                ImportedPollingStationId = pollingStation.Id,
                ImportedPollingStation = pollingStation,
                Locality = assignedAddress.Locality,
                HouseNumbers = assignedAddress.HouseNumbers,
                Remarks = assignedAddress.Remarks,
                Street = assignedAddress.Street,
                StreetCode = assignedAddress.StreetCode
            };
        }

        public async Task<Result<ImportedPollingStationModel>> GetImportedPollingStationById(Guid jobId, int importedPollingStationId)
        {
            var result = await Result.Try(async () =>
            {
                var pollingStation = await _context.ImportedPollingStations
                    .Where(x => x.Id == importedPollingStationId)
                    .Where(x => x.JobId == jobId.ToString())
                    .Select(ps => new ImportedPollingStationModel
                    {
                        Id = ps.Id,
                        Address = ps.Address,
                        Longitude = ps.Longitude,
                        Latitude = ps.Latitude,
                        County = ps.County,
                        PollingStationNumber = ps.PollingStationNumber,
                        Locality = ps.Locality,
                        Institution = ps.Institution,
                        JobId = ps.JobId,
                        ResolvedAddressStatus = ps.ResolvedAddressStatus,
                        FailMessage = ps.FailMessage
                    })
                    .FirstOrDefaultAsync();

                return pollingStation;
            });

            return result;
        }

        public async Task<Result> DeleteImportedPollingStation(Guid jobId, int pollingStationId)
        {
            var result = await Result.Try(async () =>
            {
                var pollingStation = await _context.ImportedPollingStations
                    .Where(x => x.Id == pollingStationId && x.JobId == jobId.ToString())
                    .FirstOrDefaultAsync();


                if (pollingStation == null)
                {
                    throw new Exception($"Could not find specified imported polling station for jobId={jobId} and psId = {pollingStationId}");
                }

                _context.ImportedPollingStations.Remove(pollingStation);

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
                        Locality = assignedAddress.Locality,
                        HouseNumbers = assignedAddress.HouseNumbers,
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
