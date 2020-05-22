using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VotRomania.Commands;
using VotRomania.Models;
using VotRomania.Stores;

namespace VotRomania.CommandsHandlers
{
    public class PollingStationCommandHandler : IRequestHandler<AddPollingStation, (bool isSuccess, string errorMessage, int pollingStationId)>,
        IRequestHandler<DeletePollingStation, (bool isSuccess, string errorMessage)>,
        IRequestHandler<UpdatePollingStation, (bool isSuccess, string errorMessage)>
    {
        private readonly IPollingStationsRepository _repository;

        public PollingStationCommandHandler(IPollingStationsRepository repository)
        {
            _repository = repository;
        }
        public async Task<(bool isSuccess, string errorMessage, int pollingStationId)> Handle(AddPollingStation request, CancellationToken cancellationToken)
        {
            var pollingStation = new PollingStationModel()
            {
                County = request.PollingStation.County,
                Locality = request.PollingStation.Locality,
                PollingStationNumber = request.PollingStation.PollingStationNumber,
                Address = request.PollingStation.Address,
                Institution = request.PollingStation.Institution,
                Latitude = request.PollingStation.Latitude,
                Longitude = request.PollingStation.Longitude,
            };

            return await _repository.AddPollingStationAsync(pollingStation);
        }

        public async Task<(bool isSuccess, string errorMessage)> Handle(DeletePollingStation request, CancellationToken cancellationToken)
        {
            return await _repository.DeletePollingStationAsync(request.PollingStationId);
        }

        public async Task<(bool isSuccess, string errorMessage)> Handle(UpdatePollingStation request, CancellationToken cancellationToken)
        {

            var pollingStation = new PollingStationModel()
            {
                Id = request.PollingStationId,
                County = request.PollingStation.County,
                Locality = request.PollingStation.Locality,
                PollingStationNumber = request.PollingStation.PollingStationNumber,
                Address = request.PollingStation.Address,
                Institution = request.PollingStation.Institution,
                Latitude = request.PollingStation.Latitude,
                Longitude = request.PollingStation.Longitude,
            };
            return await _repository.UpdatePollingStationAsync(pollingStation);

        }
    }
}