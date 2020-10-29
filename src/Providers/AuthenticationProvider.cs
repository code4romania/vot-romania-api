using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VotRomania.Models;
using VotRomania.Options;

namespace VotRomania.Providers
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly ILogger<AuthenticationProvider> _logger;
        private readonly AuthSettingOptions _authSettings;
        private readonly IUserProvider _userProvider;

        public AuthenticationProvider(
            ILogger<AuthenticationProvider> logger,
            IOptions<AuthSettingOptions> authSettingOptions,
            IUserProvider userProvider)
        {

            _authSettings = authSettingOptions.Value;

            _logger = logger;
            _userProvider = userProvider;
        }

        public async Task<TokenResponseModel?> CreateUserTokenAsync(string username, string password)
        {
            var userInfo = await _userProvider.GetUserAsync(username, password);
            if (userInfo == null)
            {
                _logger.LogWarning("Could not find specific user");
                return null;
            }

            var expires = DateTime.Now.AddDays(365);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSettings.Secret);

            var identityClaims = new List<Claim>(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, userInfo.Name),
            });

            if (userInfo.Scopes != null)
            {
                foreach (var scope in userInfo.Scopes)
                    identityClaims.Add(new Claim("scope", scope));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(identityClaims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenDto = new TokenResponseModel
            {
                UserName = userInfo.UserName,
                Name = userInfo.Name,
                Scopes = userInfo.Scopes,
                Expires = expires,
                Token = tokenHandler.WriteToken(token)
            };
            return tokenDto;
        }



    }
}