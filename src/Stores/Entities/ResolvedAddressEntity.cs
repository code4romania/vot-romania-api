namespace VotRomania.Stores.Entities
{
    public class ResolvedAddressEntity
    {
        public int Id { get; set; }
        public string County { get; set; }
        public string Locality { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
