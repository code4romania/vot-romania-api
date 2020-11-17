namespace VotRomania.Models
{
    public class ResolvedAddressModel
    {
        public string County { get; set; }
        public string Locality { get; set; }
        public string Street { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
