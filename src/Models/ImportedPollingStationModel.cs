using VotRomania.Stores.Entities;

namespace VotRomania.Models
{
    public class ImportedPollingStationModel
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

        public AssignedAddressModel[]? AssignedAddresses;
    }
}