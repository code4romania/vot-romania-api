using System.Threading.Tasks;
using VotRomania.Services.Location.HereMaps;

namespace VotRomania.Services.Location
{
    public interface IAddressLocationSearchService
    {
        Task<LocationSearchResult> FindCoordinates(string county, string fullAddress);
    }
}