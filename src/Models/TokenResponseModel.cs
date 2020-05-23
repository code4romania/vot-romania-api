using System;

namespace VotRomania.Models
{
    public class TokenResponseModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string[] Scopes { get; set; }
        public DateTime Expires { get; set; }
        public string Token { get; set; }
    }
}