using System.Collections.Generic;

namespace VotRomania.Models
{
    public class PollingStationModel
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string County { get; set; }
        public string Locality { get; set; }
        public string PollingStationNumber { get; set; }
        public string Institution { get; set; }
        public string Address { get; set; }
        public IList<AssignedAddressModel> AssignedAddresses { get; set; }
    }
}