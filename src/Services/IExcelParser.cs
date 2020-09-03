using System;
using System.Collections.Generic;
using System.Data;
using CSharpFunctionalExtensions;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VotRomania.Models;

namespace VotRomania.Services
{
    public interface IExcelParser
    {
        Result<List<PollingStationModel>> ParsePollingStations(IFormFile requestFile);
    }

    public class ExcelParser : IExcelParser
    {
        private readonly ILogger<ExcelParser> _logger;

        public ExcelParser(ILogger<ExcelParser> logger)
        {
            _logger = logger;
        }
        public Result<List<PollingStationModel>> ParsePollingStations(IFormFile requestFile)
        {
            var parseResult = Result.Try(() =>
            {
                DataSet result;

                using (var reader = ExcelReaderFactory.CreateReader(requestFile.OpenReadStream()))
                {
                    result = reader.AsDataSet();
                }

                var pollingStationsData = result.Tables[0];
                var pollingStations = new List<PollingStationModel>();

                PollingStationModel? currentPollingStation = null;
                int index = 1;
                do
                {
                    DataRow row = pollingStationsData.Rows[index];

                    var pollingStationNumber = GetString(row[4]);
                    if (string.IsNullOrEmpty(pollingStationNumber))
                    {
                        do
                        {
                            AssignedAddressModel assignedAddress = new AssignedAddressModel
                            {
                                HouseNumbers = GetString(pollingStationsData.Rows[index][11]),
                                Remarks = GetString(pollingStationsData.Rows[index][12]),
                                Street = GetString(pollingStationsData.Rows[index][10]),
                                StreetCode = GetString(pollingStationsData.Rows[index][9]),

                            };
                            currentPollingStation?.AssignedAddresses.Add(assignedAddress);
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
                            Locality = GetString(row[8]),
                            Institution = GetString(row[5]),
                            Address = GetString(row[6]),
                            AssignedAddresses = new List<AssignedAddressModel>()
                        };

                        pollingStations.Add(currentPollingStation);
                    }

                    ++index;

                } while (index < pollingStationsData.Rows.Count);

                return pollingStations;
            }, exception => LogException(exception, $"Error in method {nameof(ParsePollingStations)}"));

            return parseResult;
        }

        private string LogException(Exception exception, string? message = null)
        {
            var exceptionMessage = string.IsNullOrWhiteSpace(message) ? exception.Message : message;
            _logger.LogError(exception, exceptionMessage);
            return exceptionMessage;
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