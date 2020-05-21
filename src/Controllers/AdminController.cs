using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VotRomania.Commands;
using VotRomania.Models;

namespace VotRomania.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [Route("polling-station")]
        [SwaggerOperation(Summary = "Adds a polling station")]
        [SwaggerResponse(200, "Id of new polling station", typeof(int))]
        [SwaggerResponse(500, "Something went wrong when adding or updating a polling station", typeof(ProblemDetails))]
        public async Task<IActionResult> AddPollingStationAsync([FromBody]PollingStationUploadModel pollingStation)
        {
            var result = await _mediator.Send(new AddNewPollingStation(pollingStation));

            if (!result.isSuccess)
            {
                return Problem(result.errorMessage);
            }

            return Ok(result.pollingStationId);
        }

        [HttpPost]
        [Route("polling-station/{id}")]
        [SwaggerOperation(Summary = "Updates a polling station")]
        [SwaggerResponse(200, "Latest data", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when adding or updating a polling station", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdatePollingStationAsync([FromRoute] int id, [FromBody]PollingStationUploadModel pollingStation)
        {
            var result = await _mediator.Send(new UpdatePollingStation(id, pollingStation));

            if (!result.isSuccess)
            {
                return Problem(result.errorMessage);
            }

            return Ok();
        }


        [HttpDelete]
        [Route("polling-station/{id}")]
        [SwaggerOperation(Summary = "Deletes a polling station")]
        [SwaggerResponse(200, "Operation response", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when deleting a polling station", typeof(ProblemDetails))]
        public async Task<IActionResult> DeletePollingStationAsync([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeletePollingStation(id));

            if (!result.isSuccess)
            {
                return Problem(result.errorMessage);
            }

            return Ok();
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


        [HttpPost]
        [Route("content")]
        [SwaggerOperation(Summary = "Updates content")]
        [SwaggerResponse(200, "Operation response", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when updating content", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateContentAsync(ApplicationContent content)
        {
            var result = await _mediator.Send(new UpdateApplicationContent(content));

            if (!result.isSuccess)
            {
                return Problem(result.errorMessage);
            }

            return Ok();
        }

    }
}