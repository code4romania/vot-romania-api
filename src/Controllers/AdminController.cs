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
using VotRomania.Queries;

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


        [HttpPost, DisableRequestSizeLimit]
        [Route("import/upload-polling-stations")]
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

        [HttpGet]
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


        [HttpGet]
        [Route("import/current-job")]
        [SwaggerOperation(Summary = "Get current job that is being processed.")]
        [SwaggerResponse(200, "Import job details", typeof(JobStatusModel))]
        [SwaggerResponse(500, "Something went wrong when getting job details.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetCurrentJob()
        {
            var result = await _mediator.Send(new GetCurrentImportJob());
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return Problem(result.Error);
        }


        [HttpPost]
        [Route("import/cancel-job/{jobId:guid}")]
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


        [HttpPost]
        [Route("import/complete-job/{jobId:guid}")]
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


        [HttpPost]
        [Route("import/restart-job/{jobId:guid}")]
        [SwaggerOperation(Summary = "Restarts job", Description = "Restarts job")]
        [SwaggerResponse(200, "Successfully restarted job", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when restarting job.", typeof(ProblemDetails))]
        public async Task<IActionResult> RestartJob(Guid jobId)
        {
            var result = await _mediator.Send(new RestartImportJob(jobId));
            if (result.IsSuccess)
            {
                return Ok();
            }

            return Problem(result.Error);
        }

        [HttpGet]
        [Route("import/{jobId:guid}/imported-polling-stations")]
        [SwaggerOperation(Summary = "Search imported polling stations by specific criteria")]
        [SwaggerResponse(200, "Imported polling stations found.", typeof(PagedResult<PollingStationModel>))]
        [SwaggerResponse(404, "No polling stations found.", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when searching.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetFilteredImportedPollingStations(
              [FromRoute]Guid jobId,
              [FromQuery] PaginationQuery pagination,
              [FromQuery] ImportedPollingStationsQuery query)
        {

            var result = await _mediator.Send(new SearchImportedPollingStations(jobId, pagination, query));

            if (result.IsFailure)
            {
                return Problem(result.Error);
            }

            return Ok(result.Value);
        }


        [HttpGet]
        [Route("import/{jobId:guid}/imported-polling-stations/{id}")]
        [SwaggerResponse(200, "Imported polling station details.", typeof(PagedResult<PollingStationModel>))]
        [SwaggerResponse(401)]
        [SwaggerResponse(404, "No polling station found.", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when searching.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetImportedPollingStationAsync([FromRoute]Guid jobId, [FromRoute] int id)
        {
            var result = await _mediator.Send(new GetImportedPollingStationById(jobId, id));


            if (result.IsFailure)
            {
                return Problem(result.Error);
            }

            if (result.Value == null) return NotFound();

            return Ok(result.Value);
        }

        [HttpPost]
        [Route("import/{jobId:guid}/imported-polling-stations")]
        [SwaggerOperation(Summary = "Adds an imported polling station")]
        [SwaggerResponse(200, "Id of new imported polling station", typeof(int))]
        [SwaggerResponse(401)]
        [SwaggerResponse(500, "Something went wrong when adding an imported polling station", typeof(ProblemDetails))]
        public async Task<IActionResult> AddPollingStationAsync([FromRoute]Guid jobId, [FromBody]ImportedPollingStationUploadModel pollingStation)
        {
            var result = await _mediator.Send(new AddImportedPollingStation(jobId, pollingStation));

            if (result.IsFailure)
            {
                return Problem(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Route("import/{jobId:guid}/imported-polling-stations/{id}")]
        [SwaggerOperation(Summary = "Updates an imported polling station")]
        [SwaggerResponse(200, "Latest data", typeof(void))]
        [SwaggerResponse(401)]
        [SwaggerResponse(500, "Something went wrong when adding or updating an Imported polling station", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateImportedPollingStationAsync([FromRoute]Guid jobId, [FromRoute] int id, [FromBody]ImportedPollingStationUploadModel pollingStation)
        {
            var result = await _mediator.Send(new UpdateImportedPollingStation(jobId, id, pollingStation));

            if (result.IsFailure)
            {
                return Problem(result.Error);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("import/{jobId:guid}/imported-polling-stations/{id}")]
        [SwaggerOperation(Summary = "Deletes an imported polling station")]
        [SwaggerResponse(200, "Operation response", typeof(void))]
        [SwaggerResponse(401)]
        [SwaggerResponse(500, "Something went wrong when deleting an imported polling station", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteImportedPollingStationAsync([FromRoute]Guid jobId, [FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteImportedPollingStation(jobId, id));

            if (result.IsFailure)
            {
                return Problem(result.Error);
            }

            return Ok();
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