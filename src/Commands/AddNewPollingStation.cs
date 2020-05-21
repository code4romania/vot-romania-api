using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class AddNewPollingStation : IRequest<(bool isSuccess, string errorMessage, int pollingStationId)>
    {
        public PollingStationUploadModel PollingStation { get; }

        public AddNewPollingStation(PollingStationUploadModel pollingStation)
        {
            PollingStation = pollingStation;
        }
    }
}