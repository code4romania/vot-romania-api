using System.Threading.Tasks;

namespace VotRomania.Providers
{
    public interface IUserProvider
    {
        Task<UserModel> GetUserAsync(string username, string password);
    }
}