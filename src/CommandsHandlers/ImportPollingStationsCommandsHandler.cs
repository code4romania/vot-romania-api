using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Http;
using VotRomania.Commands;
using VotRomania.Models;
using VotRomania.Stores;
using VotRomania.Stores.Entities;

namespace VotRomania.CommandsHandlers
{
    public class ImportPollingStationsCommandsHandler : IRequestHandler<StartUploadNewPollingStations, Result<Guid>>
    {
        private readonly VotRomaniaContext _context;

        public ImportPollingStationsCommandsHandler(VotRomaniaContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(StartUploadNewPollingStations request, CancellationToken cancellationToken)
        {
            var result = await ParsePollingStations(request.File)
                .MapWithTransactionScope(async ps =>
                {
                    return await InsertPollingStationToTempTable(ps)
                          .Map(_ => InsertInJobTable(request.File))
                          .Tap(jobId => NotifyBackgroundJob(jobId));
                })
                .Tap(x => x);

            return new Result<Guid>();


        }

        private async Task<Result<Guid>> NotifyBackgroundJob(Guid jobId)
        {
            throw new NotImplementedException();
        }

        private async Task<Result<Guid>> InsertInJobTable(IFormFile requestFile)
        {

            return await Result.Try(async () =>
            {
                Guid jobId = Guid.NewGuid();
                using (var ms = new MemoryStream())
                {
                    requestFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string encodedFile = Convert.ToBase64String(fileBytes);

                    await _context.UploadJobs.AddAsync(new UploadJobsEntity()
                    {
                        FileName = requestFile.FileName,
                        Base64File = encodedFile,
                        JobId = jobId.ToString(),
                        JobStatus = JobStatus.NotStarted
                    });
                    return jobId;
                }
            });
        }

        private async Task<Result> InsertPollingStationToTempTable(List<PollingStationModel> pollingStationModels)
        {
            throw new NotImplementedException();
        }


        private Result<List<PollingStationModel>> ParsePollingStations(IFormFile requestFile)
        {
            return Result.Try(() =>
            {
                DataSet result;

                using (var reader = ExcelReaderFactory.CreateReader(requestFile.OpenReadStream()))
                {
                    result = reader.AsDataSet();
                }

                var pollingStationsData = result.Tables[0];
                var pollingStations = new List<PollingStationModel>();

                PollingStationModel currentPollingStation = null;
                int index = 1;
                do
                {
                    DataRow row = pollingStationsData.Rows[index];

                    var pollingStationNumber = GetString(row[4]);
                    if (string.IsNullOrEmpty(pollingStationNumber))
                    {
                        do
                        {
                            AssignedAddresses assignedAddress = new AssignedAddresses
                            {
                                HouseNumbers = GetString(pollingStationsData.Rows[index][12]),
                                Locality = GetString(pollingStationsData.Rows[index][9]),
                                Remarks = GetString(pollingStationsData.Rows[index][13]),
                                Street = GetString(pollingStationsData.Rows[index][11]),
                                StreetCode = GetString(pollingStationsData.Rows[index][10]),

                            };
                            currentPollingStation.AssignedAddresses.Add(assignedAddress);
                            ++index;

                        } while (index < pollingStationsData.Rows.Count &&
                                 string.IsNullOrEmpty(GetString(pollingStationsData.Rows[index][4])));

                        --index;
                    }
                    else
                    {
                        currentPollingStation = new PollingStationModel
                        {
                            County = GetString(row[0]),
                            PollingStationNumber = pollingStationNumber,
                            Locality = GetString(row[9]),
                            Institution = GetString(row[6]),
                            Address = GetString(row[7]),
                            AssignedAddresses = new List<AssignedAddresses>()
                        };

                        pollingStations.Add(currentPollingStation);
                    }

                    ++index;

                } while (index < pollingStationsData.Rows.Count);

                return pollingStations;
            });

        }

        private static string GetString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var text = value.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return text.Trim();
        }
    }
}
