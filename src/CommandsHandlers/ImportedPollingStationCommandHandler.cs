using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Commands;
using VotRomania.Models;
using VotRomania.Stores;
using VotRomania.Stores.Entities;

namespace VotRomania.CommandsHandlers
{
    public class ImportedPollingStationCommandHandler :
        IRequestHandler<AddImportedPollingStation, Result<int>>,
        IRequestHandler<DeleteImportedPollingStation, Result>,
        IRequestHandler<UpdateImportedPollingStation, Result>
    {
        private readonly IImportedPollingStationsRepository _repository;

        public ImportedPollingStationCommandHandler(IImportedPollingStationsRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<int>> Handle(AddImportedPollingStation request, CancellationToken cancellationToken)
        {
            var pollingStation = new ImportedPollingStationModel
            {
                County = request.PollingStation.County,
                Locality = request.PollingStation.Locality,
                PollingStationNumber = request.PollingStation.PollingStationNumber,
                Address = request.PollingStation.Address,
                Institution = request.PollingStation.Institution,
                ResolvedAddressStatus = request.PollingStation.ResolvedAddressStatus ?? ResolvedAddressStatusType.NotProcessed,
                Latitude = request.PollingStation.Latitude,
                Longitude = request.PollingStation.Longitude,
                JobId = request.JobId.ToString(),
                AssignedAddresses = request.PollingStation.AssignedAddresses
            };

            return await _repository.AddPollingStation(request.JobId, pollingStation);
        }

        public async Task<Result> Handle(DeleteImportedPollingStation request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteImportedPollingStation(request.JobId, request.PollingStationId);
        }

        public async Task<Result> Handle(UpdateImportedPollingStation request, CancellationToken cancellationToken)
        {

            var pollingStation = new ImportedPollingStationModel
            {
                Id = request.PollingStationId,
                County = request.PollingStation.County,
                Locality = request.PollingStation.Locality,
                PollingStationNumber = request.PollingStation.PollingStationNumber,
                Address = request.PollingStation.Address,
                Institution = request.PollingStation.Institution,
                Latitude = request.PollingStation.Latitude,
                Longitude = request.PollingStation.Longitude,
                ResolvedAddressStatus = request.PollingStation.ResolvedAddressStatus ?? ResolvedAddressStatusType.NotProcessed,
                FailMessage = request.PollingStation.FailMessage,
                JobId = request.JobId.ToString(),
                AssignedAddresses = request.PollingStation.AssignedAddresses
            };

            return await _repository.UpdateImportedPollingStation(request.JobId, pollingStation);
        }
    }
}