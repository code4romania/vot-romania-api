using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class GetPollingStations : IRequest<PollingStationsGroupModel[]>
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GetPollingStations(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
