using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VotRomania.Options;

namespace VotRomania.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly IOptions<ApplicationUsersOptions> _usersOptions;

        public UserProvider(IOptions<ApplicationUsersOptions> usersOptions)
        {
            _usersOptions = usersOptions;
        }

        public Task<UserModel> GetUserAsync(string username, string password)
        {
            var users = _usersOptions.Value;
            if (users?.Users == null)
                return Task.FromResult<UserModel>(null);

            var user = users.Users.SingleOrDefault(x => x.UserName == username && x.Password == password);
            if (user == null)
                return Task.FromResult<UserModel>(null);

            var userInfo = new UserModel
            {
                UserName = user.UserName,
                Name = user.Name,
                Scopes = user.Scopes
            };

            return Task.FromResult(userInfo);
        }
    }

    public class UserModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string[] Scopes { get; set; }
    }

}

