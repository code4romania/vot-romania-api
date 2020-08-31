using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class UpdatePollingStation : IRequest<(bool isSuccess, string errorMessage)>
    {
        public int PollingStationId { get; }
        public PollingStationUploadModel PollingStation { get; }

        public UpdatePollingStation(int pollingStationId, PollingStationUploadModel pollingStation)
        {
            PollingStationId = pollingStationId;
            PollingStation = pollingStation;
        }
    }
}