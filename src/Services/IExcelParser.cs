using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                List<ExcelRowModel> parsedRows = new List<ExcelRowModel>();

                int index = 1;
                string county = string.Empty;
                string pollingStationLocality = string.Empty;
                string code = string.Empty;
                string officeNr = string.Empty;
                string psNumber = string.Empty;
                string institution = string.Empty;
                string address = string.Empty;
                string locality = string.Empty;
                string streetCode = string.Empty;
                string street = string.Empty;
                string houseNumbers = string.Empty;
                string remarks = string.Empty;
                do
                {
                    DataRow row = pollingStationsData.Rows[index];

                    county = GetStringOrDefault(row[0], county);
                    code = GetStringOrDefault(row[2], code);
                    officeNr = GetStringOrDefault(row[3], officeNr);
                    psNumber = GetStringOrDefault(row[4], psNumber);
                    institution = GetStringOrDefault(row[5], institution);
                    address = GetStringOrDefault(row[6], address);
                    locality = GetStringOrDefault(row[8], locality);
                    streetCode = GetStringOrDefault(pollingStationsData.Rows[index][9], streetCode);
                    street = GetStringOrDefault(pollingStationsData.Rows[index][10], street);
                    houseNumbers = GetStringOrDefault(pollingStationsData.Rows[index][11], houseNumbers);
                    remarks = GetStringOrDefault(pollingStationsData.Rows[index][12], remarks);

                    if (address.StartsWith("Loc. ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        address = address.Replace("Loc. ", "", StringComparison.InvariantCultureIgnoreCase);
                    }

                    pollingStationLocality = address.Split(",").FirstOrDefault();

                    parsedRows.Add(new ExcelRowModel
                    {
                        County = Counties.GetByCode(county),
                        PollingStationLocality = pollingStationLocality,
                        Code = code,
                        OfficeNr = officeNr,
                        PsNumber = psNumber,
                        Institution = institution,
                        Address = address,
                        AssignedAddressLocality = locality,
                        StreetCode = streetCode,
                        Street = street,
                        HouseNumbers = houseNumbers,
                        Remarks = remarks,
                    });

                    ++index;

                } while (index < pollingStationsData.Rows.Count);

                pollingStations = parsedRows.GroupBy(
                        r => new { r.County, r.PsNumber, r.PollingStationLocality, r.Institution, r.Address },
                        x => x,
                        (key, g) => new PollingStationModel()
                        {
                            County = key.County,
                            Locality = key.PollingStationLocality,
                            PollingStationNumber = key.PsNumber,
                            Institution = key.Institution,
                            Address = key.Address,
                            AssignedAddresses = g.Select(x => new AssignedAddressModel()
                            {
                                Locality = x.AssignedAddressLocality,
                                StreetCode = x.StreetCode,
                                Remarks = x.Remarks,
                                Street = x.Street,
                                HouseNumbers = x.HouseNumbers

                            }).ToList()

                        })
                    .ToList();

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

        private static string GetStringOrDefault(object value, string defaultValue)
        {
            if (value == null)
            {
                return defaultValue;
            }

            var text = value.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return defaultValue;
            }

            return text.Trim();
        }


    }

    public class ExcelRowModel
    {
        public string County { get; set; }
        public string PollingStationLocality { get; set; }
        public string Code { get; set; }
        public string OfficeNr { get; set; }
        public string PsNumber { get; set; }
        public string Institution { get; set; }
        public string Address { get; set; }
        public string AssignedAddressLocality { get; set; }
        public string StreetCode { get; set; }
        public string Street { get; set; }
        public string HouseNumbers { get; set; }
        public string Remarks { get; set; }
    }
}