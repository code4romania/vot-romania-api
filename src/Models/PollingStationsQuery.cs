using System;
using Microsoft.AspNetCore.Mvc;
using VotRomania.Stores.Entities;

namespace VotRomania.Models
{
    public class PollingStationsQuery
    {
        [FromQuery(Name = "pollingStationId")] public int? PollingStationId { get; set; }
        [FromQuery(Name = "County")] public string County { get; set; }
        [FromQuery(Name = "locality")] public string Locality { get; set; }
        [FromQuery(Name = "pollingStationNumber")] public string PollingStationNumber { get; set; }
        [FromQuery(Name = "institution")] public string Institution { get; set; }
        [FromQuery(Name = "address")] public string Address { get; set; }
    }

    public class ImportedPollingStationsQuery : PollingStationsQuery
    {
        [FromQuery(Name = "jobId")] public Guid? JobId { get; set; }
        [FromQuery(Name = "resolvedAddressStatus")] public ResolvedAddressStatusType? ResolvedAddressStatus { get; set; }
    }
}