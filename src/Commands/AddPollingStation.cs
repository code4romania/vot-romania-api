using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class AddPollingStation : IRequest<(bool isSuccess, string errorMessage, int pollingStationId)>
    {
        public PollingStationUploadModel PollingStation { get; }

        public AddPollingStation(PollingStationUploadModel pollingStation)
        {
            PollingStation = pollingStation;
        }
    }
}