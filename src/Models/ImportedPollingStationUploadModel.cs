using System.ComponentModel.DataAnnotations;
using VotRomania.Stores.Entities;

namespace VotRomania.Models
{
    public class ImportedPollingStationUploadModel
    {
        [Required] public string County { get; set; }
        [Required] public string Locality { get; set; }
        [Required] public string PollingStationNumber { get; set; }
        [Required] public string Institution { get; set; }
        [Required] public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public ResolvedAddressStatusType? ResolvedAddressStatus { get; set; }
        public string? FailMessage { get; set; }
        public AssignedAddressModel[]? AssignedAddresses { get; set; }


    }
}