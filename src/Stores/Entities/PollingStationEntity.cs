using System.Collections.Generic;

namespace VotRomania.Stores.Entities
{
    public class PollingStationEntity
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string County { get; set; }
        public string Locality { get; set; }
        public string PollingStationNumber { get; set; }
        public string Institution { get; set; }
        public string Address { get; set; }
        public ICollection<PollingStationAddressEntity> PollingStationAddresses { get; set; }
    }

    public class PollingStationAddressEntity
    {
        public int Id { get; set; }
        public string Locality { get; set; }
        public string StreetCode { get; set; }
        public string Street { get; set; }
        public string HouseNumbers { get; set; }
        public string Remarks { get; set; }
        public PollingStationEntity PollingStation { get; set; }
        public int PollingStationId { get; set; }
    }
}