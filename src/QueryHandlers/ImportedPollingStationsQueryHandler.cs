using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Models;
using VotRomania.Queries;
using VotRomania.Stores;

namespace VotRomania.QueryHandlers
{
    public class ImportedPollingStationsQueryHandler :
        IRequestHandler<SearchImportedPollingStations, Result<PagedResult<ImportedPollingStationModel>>>,
        IRequestHandler<GetImportedPollingStationById, Result<ImportedPollingStationModel>>
    {
        private readonly IImportedPollingStationsRepository _repository;

        public ImportedPollingStationsQueryHandler(IImportedPollingStationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedResult<ImportedPollingStationModel>>> Handle(SearchImportedPollingStations request, CancellationToken cancellationToken)
        {
            var pollingStations = await _repository.GetImportedPollingStationsAsync(request.JobId, request.Query, request.Pagination);

            return pollingStations;
        }

        public async Task<Result<ImportedPollingStationModel>> Handle(GetImportedPollingStationById request, CancellationToken cancellationToken)
        {
            var pollingStation = await _repository.GetImportedPollingStationById(request.JobId, request.PollingStationId);

            return pollingStation;
        }
    }
}