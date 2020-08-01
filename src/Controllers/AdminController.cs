using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VotRomania.Commands;
using VotRomania.Models;
using VotRomania.Providers;

namespace VotRomania.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/admin")]
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

        // TODO: remove anonymous
        [AllowAnonymous]
        [HttpPost, DisableRequestSizeLimit]
        [Route("import/polling-stations")]
        [Consumes("multipart/form-data")]

        [SwaggerOperation(Summary = "Upload polling stations from excel file")]
        [SwaggerResponse(200, "Import job id.", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when importing polling stations.", typeof(ProblemDetails))]
        public async Task<IActionResult> Upload([FromForm]PollingStationsImportModel model)
        {
            var result = await _mediator.Send(new StartImportNewPollingStations(model.FormFile));
            if (result.IsSuccess)
            {
                return Ok(new { jobId = result.Value });
            }

            return Problem(result.Error);
        }

        // TODO: remove anonymous
        [AllowAnonymous]
        [HttpGet, DisableRequestSizeLimit]
        [Route("import/job-status")]
        [SwaggerOperation(Summary = "Get details for a specific job")]
        [SwaggerResponse(200, "Import job details", typeof(JobStatusModel))]
        [SwaggerResponse(500, "Something went wrong when getting job details.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetJobStatus(Guid jobId)
        {
            var result = await _mediator.Send(new GetImportJobStatus(jobId));
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return Problem(result.Error);
        }

        // TODO: remove anonymous
        [AllowAnonymous]
        [HttpGet, DisableRequestSizeLimit]
        [Route("import/cancel-job")]
        [SwaggerOperation(Summary = "Cancels a specific job")]
        [SwaggerResponse(200, "Successfully cancelled a job", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when cancelling job.", typeof(ProblemDetails))]
        public async Task<IActionResult> CancelJob(Guid jobId)
        {
            var result = await _mediator.Send(new CancelImportJob(jobId));
            if (result.IsSuccess)
            {
                return Ok();
            }

            return Problem(result.Error);
        }

        // TODO: remove anonymous
        [AllowAnonymous]
        [HttpGet, DisableRequestSizeLimit]
        [Route("import/complete-job")]
        [SwaggerOperation(Summary = "Completes current job", Description = "Please note that this operations will add imported polling stations in main polling stations table and remove old ones! Operation will fail if job is not completed and there are some polling stations with unresolved addresses.")]
        [SwaggerResponse(200, "Successfully completes import job", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when completing job.", typeof(ProblemDetails))]
        public async Task<IActionResult> CompleteJob(Guid jobId)
        {
            var result = await _mediator.Send(new CompleteImportJob(jobId));
            if (result.IsSuccess)
            {
                return Ok();
            }

            return Problem(result.Error);
        }


        [AllowAnonymous]
        [HttpPost("login")]
        [Consumes("application/json")]
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