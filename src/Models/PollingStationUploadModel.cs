using System.ComponentModel.DataAnnotations;

namespace VotRomania.Models
{
    public class PollingStationUploadModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required] public string County { get; set; }
        [Required] public string Locality { get; set; }
        [Required] public string PollingStationNumber { get; set; }
        [Required] public string Institution { get; set; }
        [Required] public string Address { get; set; }
    }
}