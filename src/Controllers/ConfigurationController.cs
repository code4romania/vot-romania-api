using System;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VotRomania.Options;

namespace VotRomania.Controllers
{
    [AllowAnonymous]
    public class ConfigurationController : Controller
    {
        private readonly HereMapsOptions _hereOptions;

        public ConfigurationController(IOptions<HereMapsOptions> hereOptions)
        {
            _hereOptions = hereOptions?.Value ?? throw new ArgumentNullException(nameof(hereOptions));
        }

        [HttpGet]
        [Route("configuration/appConfig.js")]
        public ContentResult GetConfiguration()
        {
            string hereMapsToken = _hereOptions.Token;

            var sb = new StringBuilder();
            sb.Append($"var hereMapsToken = '{hereMapsToken}'");

            return Content(sb.ToString(), "application/javascript");
        }
    }
}
