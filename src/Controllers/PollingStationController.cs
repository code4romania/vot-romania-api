using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VotRomania.Commands;
using VotRomania.Models;
using VotRomania.Queries;

namespace VotRomania.Controllers
{
    [Authorize(Policy = "polling-stations")]
    [ApiController]
    [Route("api/polling-station")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class PollingStationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PollingStationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Search polling stations by specific criteria")]
        [SwaggerResponse(200, "Polling stations found.", typeof(PagedResult<PollingStationModel>))]
        [SwaggerResponse(404, "No polling stations found.", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when searching.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetFilteredPollingStations(
            [FromQuery] PaginationQuery pagination,
            [FromQuery] PollingStationsQuery query)
        {
            var result = await _mediator.Send(new SearchPollingStations(pagination, query));

            if (result.IsFailure)
            {
                return Problem(result.Error);
            }

            return Ok(result.Value);
        }


        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(200, "Polling station details.", typeof(PagedResult<PollingStationModel>))]
        [SwaggerResponse(401)]
        [SwaggerResponse(404, "No polling station found.", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when searching.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetPollingStationAsync([FromRoute] int id)
        {
            var pollingStation = await _mediator.Send(new GetPollingStationById(id));

            if (pollingStation == null)
            {
                return NotFound();
            }

            return Ok(pollingStation);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adds a polling station")]
        [SwaggerResponse(200, "Id of new polling station", typeof(int))]
        [SwaggerResponse(401)]
        [SwaggerResponse(500, "Something went wrong when adding or updating a polling station", typeof(ProblemDetails))]
        public async Task<IActionResult> AddPollingStationAsync([FromBody]PollingStationUploadModel pollingStation)
        {
            var result = await _mediator.Send(new AddPollingStation(pollingStation));

            if (!result.isSuccess)
            {
                return Problem(result.errorMessage);
            }

            return Ok(result.pollingStationId);
        }

        [HttpPost]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Updates a polling station")]
        [SwaggerResponse(200, "Latest data", typeof(void))]
        [SwaggerResponse(401)]
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
        [Route("{id}")]
        [SwaggerOperation(Summary = "Deletes a polling station")]
        [SwaggerResponse(200, "Operation response", typeof(void))]
        [SwaggerResponse(401)]
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

        [HttpGet]
        [AllowAnonymous]
        [Route("near-me")]
        [SwaggerOperation(Summary = "Gets nearest polling stations for a specific geo point")]
        [SwaggerResponse(200, "Polling stations found.", typeof(PollingStationsGroupModel[]))]
        [SwaggerResponse(404, "No polling stations found.", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when searching.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetNearbyPollingStations([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var data = await _mediator.Send(new GetNearbyPollingStations(latitude, longitude));

            if (data == null || data.Length == 0)
            {
                return NotFound();
            }

            return Ok(data);
        }

    }
}