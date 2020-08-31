using MediatR;

namespace VotRomania.Commands
{
    public class DeletePollingStation : IRequest<(bool isSuccess, string errorMessage)>
    {
        public int PollingStationId { get; }

        public DeletePollingStation(int pollingStationId)
        {
            PollingStationId = pollingStationId;
        }
    }
}