using VotRomania.Stores.Entities;

namespace VotRomania.Services.Location.HereMaps
{
    public class LocationSearchResult
    {
        public ResolvedAddressStatusType OperationStatus { get; set; }
        public string ErrorMessage { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}