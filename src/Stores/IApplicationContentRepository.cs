using System.Threading.Tasks;
using VotRomania.Models;

namespace VotRomania.Stores
{
    public interface IApplicationContentRepository
    {
        Task<ApplicationContentModel[]> GetApplicationContentAsync(Language? language = null);
        Task<(bool isSuccess, string errorMessage)> AddApplicationContentAsync(ApplicationContentModel content);
        Task<(bool isSuccess, string errorMessage)> DeleteApplicationContentAsync(Language language);
        Task<(bool isSuccess, string errorMessage)> UpdateApplicationContentAsync(ApplicationContentModel content);
    }
}