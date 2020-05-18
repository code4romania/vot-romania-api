namespace VotRomania.Models
{
    public class PollingStationsGroupModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PollingStationModel[] PollingStations { get; set; }
    }
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
    }
}