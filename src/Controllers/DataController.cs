using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VotRomania.Models;
using VotRomania.Queries;
using VotRomania.Stores;

namespace VotRomania.Controllers
{
    [ApiController]
    [Route("api")]
    public class DataController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DataController> _logger;
        private readonly IPollingStationsRepository _repository;

        public DataController(IMediator mediator, ILogger<DataController> logger, IPollingStationsRepository repository)
        {
            _mediator = mediator;
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [Route("data")]
        public async Task<ApplicationData> Get()
        {
            var data = await _mediator.Send(new GetData());
            return data;
        }

        [HttpGet]
        [Route("polling-stations")]
        public async Task<PollingStationsGroupModel[]> GetPollingStations([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var data = await _mediator.Send(new GetPollingStations(latitude, longitude));

            return data;
        }
    }
}
