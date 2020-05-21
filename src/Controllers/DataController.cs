using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VotRomania.Models;
using VotRomania.Queries;

namespace VotRomania.Controllers
{
    [ApiController]
    [Route("api")]
    public class DataController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DataController> _logger;

        public DataController(IMediator mediator, ILogger<DataController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("content")]
        [SwaggerOperation(Summary = "Gets website content")]
        [SwaggerResponse(200, "Content.", typeof(ApplicationContent))]
        [SwaggerResponse(500, "Something went wrong when searching.", typeof(ProblemDetails))]
        public async Task<ApplicationContent> Get()
        {
            var data = await _mediator.Send(new GetData());
            return data;
        }

        [HttpGet]
        [Route("search-polling-stations")]
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

        [HttpGet]
        [Route("polling-stations")]
        [SwaggerOperation(Summary = "Search polling stations by specific criteria")]
        [SwaggerResponse(200, "Polling stations found.", typeof(PagedResult<PollingStationModel>))]
        [SwaggerResponse(404, "No polling stations found.", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when searching.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetFilteredPollingStations(
            [FromQuery] PaginationQuery pagination,
            [FromQuery] PollingStationsQuery query)
        {
            var pollingStations = await _mediator.Send(new SearchPollingStation(pagination, query));

            if (pollingStations?.Results == null || pollingStations.Results.Count == 0)
            {
                return NotFound();
            }

            return Ok(pollingStations);
        }


        [HttpGet]
        [Route("polling-station/{id}")]
        [SwaggerResponse(200, "Polling stations details.", typeof(PagedResult<PollingStationModel>))]
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
    }
}
