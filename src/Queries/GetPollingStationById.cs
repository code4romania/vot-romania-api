using MediatR;
using VotRomania.Models;

namespace VotRomania.Queries
{
    public class GetPollingStationById : IRequest<PollingStationModel>
    {
        public int PollingStationId { get; }

        public GetPollingStationById(int pollingStationId)
        {
            PollingStationId = pollingStationId;
        }
    }
}