using System.Threading.Tasks;
using VotRomania.Models;

namespace VotRomania.Services
{
    public interface IPollingStationSearchService
    {
        Task<PollingStationsGroupModel[]> GetNearestPollingStationsAsync(double latitude, double longitude);
        Task BustCache();
    }
}