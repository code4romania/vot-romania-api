namespace VotRomania.Models
{
    public class PollingStationsGroupModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PollingStationModel[] PollingStations { get; set; }
        public double Distance { get; set; }
    }
}