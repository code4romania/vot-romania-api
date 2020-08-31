using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VotRomania.Models;
using VotRomania.Providers;

namespace VotRomania.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/admin")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthenticationProvider _tokenProvider;

        public AdminController(IMediator mediator, IAuthenticationProvider tokenProvider)
        {
            _mediator = mediator;
            _tokenProvider = tokenProvider;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("upload-polling-station")]
        [SwaggerOperation(Summary = "Upload polling stations from excel file")]
        [SwaggerResponse(200, "Operation response", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when updating or adding static texts", typeof(ProblemDetails))]
        public IActionResult Upload(IFormFile file)
        {
            // todo implement
            return Ok();
        }


        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerResponse(200, "The token info", typeof(TokenResponseModel))]
        [SwaggerResponse(404, "The user was not found", typeof(ProblemDetails))]
        public async Task<IActionResult> CreateTokenAsync([FromBody, Required]TokenRequestModel model)
        {
            var userToken = await _tokenProvider.CreateUserTokenAsync(model.UserName, model.Password);
            if (userToken == null)
            {
                return Problem("User credentials are invalid");
            }

            return Ok(userToken);
        }

        [HttpPost("test-token")]
        public IActionResult TestToken()
        {
            var claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(claims);
        }

    }
}