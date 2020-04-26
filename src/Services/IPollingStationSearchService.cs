using System.Threading.Tasks;
using VotRomania.Models;

namespace VotRomania.Services
{
    public interface IPollingStationSearchService
    {
        Task<PollingStationsInfo[]> GetNearestPollingStationsAsync(double latitude, double longitude);
    }
}