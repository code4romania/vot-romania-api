using System.Threading.Tasks;
using VotRomania.Models;

namespace VotRomania.Providers
{
    public interface IAuthenticationProvider
    {
        Task<TokenResponseModel?> CreateUserTokenAsync(string username, string password);
    }
}