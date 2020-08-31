using System.Threading.Tasks;
using VotRomania.Models;

namespace VotRomania.Stores
{
    public interface IPollingStationsRepository
    {
        Task<PagedResult<PollingStationModel>> GetPollingStationsAsync(PollingStationsQuery query = null, PaginationQuery pagination = null);
        Task<PollingStationModel> GetPollingStationAsync(int pollingStationId);
        Task<(bool isSuccess, string errorMessage, int pollingStationId)> AddPollingStationAsync(PollingStationModel pollingStation);
        Task<(bool isSuccess, string errorMessage)> DeletePollingStationAsync(int pollingStationId);
        Task<(bool isSuccess, string errorMessage)> UpdatePollingStationAsync(PollingStationModel pollingStation);
    }
}