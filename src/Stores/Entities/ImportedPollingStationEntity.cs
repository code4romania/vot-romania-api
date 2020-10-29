using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace VotRomania.Stores.Entities
{
    public class ImportedPollingStationEntity
    {
        public int Id { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string County { get; set; }
        public string Locality { get; set; }
        public string PollingStationNumber { get; set; }
        public string Institution { get; set; }
        public string Address { get; set; }
        public string JobId { get; set; }
        public ResolvedAddressStatusType ResolvedAddressStatus { get; set; }
        public string? FailMessage { get; set; }
        public ICollection<ImportedPollingStationAddressEntity> AssignedAddresses { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResolvedAddressStatusType
    {
        [EnumMember(Value = "notProcessed")] NotProcessed = 1,
        [EnumMember(Value = "success")] Success = 2,
        [EnumMember(Value = "notFound")] NotFound = 3,
        [EnumMember(Value = "failed")] Failed = 4
    }

    public class ImportedPollingStationAddressEntity
    {
        public int Id { get; set; }
        public string? Locality { get; set; }
        public string? StreetCode { get; set; }
        public string? Street { get; set; }
        public string? HouseNumbers { get; set; }
        public string? Remarks { get; set; }
        public ImportedPollingStationEntity ImportedPollingStation { get; set; }
        public int ImportedPollingStationId { get; set; }
    }
}