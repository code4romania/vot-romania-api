using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace VotRomania.Controllers
{
    [AllowAnonymous]
    public class ConfigurationController : Controller
    {
        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("configuration/appConfig.js")]
        public ContentResult GetConfiguration()
        {
            string hereMapsToken = _configuration["HereMaps:Token"];

            var sb = new StringBuilder();
            sb.Append($"var hereMapsToken = '{hereMapsToken}'");

            return Content(sb.ToString(), "application/javascript");
        }
    }
}
