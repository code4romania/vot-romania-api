using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class GetNearbyPollingStations : IRequest<PollingStationsGroupModel[]>
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GetNearbyPollingStations(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
