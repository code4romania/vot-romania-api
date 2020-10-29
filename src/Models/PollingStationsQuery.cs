using Microsoft.AspNetCore.Mvc;
using VotRomania.Stores.Entities;

namespace VotRomania.Models
{
    public class PollingStationsQuery
    {
        [FromQuery(Name = "county")] public string? County { get; set; }
        [FromQuery(Name = "locality")] public string? Locality { get; set; }
        [FromQuery(Name = "pollingStationNumber")] public string? PollingStationNumber { get; set; }
        [FromQuery(Name = "institution")] public string? Institution { get; set; }
        [FromQuery(Name = "address")] public string? Address { get; set; }
    }

    public class ImportedPollingStationsQuery : PollingStationsQuery
    {
        [FromQuery(Name = "resolvedAddressStatus")] public ResolvedAddressStatusType? ResolvedAddressStatus { get; set; }
    }
}