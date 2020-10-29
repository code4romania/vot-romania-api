namespace VotRomania.Models
{
    public class AssignedAddressModel
    {
        public int Id { get; set; }
        public int PollingStationId { get; set; }
        public string? StreetCode { get; set; }
        public string? Street { get; set; }
        public string? HouseNumbers { get; set; }
        public string? Remarks { get; set; }
        public string? Locality { get; set; }
    }
}