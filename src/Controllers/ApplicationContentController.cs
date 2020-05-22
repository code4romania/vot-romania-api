using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VotRomania.Commands;
using VotRomania.Models;
using VotRomania.Providers;
using VotRomania.Queries;

namespace VotRomania.Controllers
{
    [ApiController]
    [Route("api/application-content")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ApplicationContentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ApplicationContentController> _logger;
        private readonly IDataProvider _delte;

        public ApplicationContentController(IMediator mediator, ILogger<ApplicationContentController> logger, IDataProvider delte)
        {
            _mediator = mediator;
            _logger = logger;
            _delte = delte;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Gets website content")]
        [SwaggerResponse(200, "Content", typeof(StaticTexts))]
        [SwaggerResponse(500, "Something went wrong when searching.", typeof(ProblemDetails))]
        public async Task<IActionResult> Get()
        {

            var result = await _mediator.Send(new GetApplicationContent());
            if (result.isSuccess)
            {
                return Ok(new StaticTexts { Content = result.data });
            }

            return Problem(result.errorMessage);
        }

        [HttpPost]
        [Route("{language}")]
        [SwaggerOperation(Summary = "Adds or updates content")]
        [SwaggerResponse(200, "Operation response", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when updating content", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateContentAsync([FromRoute] Language language, [FromBody]ApplicationContentModel content)
        {
            var result = await _mediator.Send(new AddOrUpdateApplicationContent(language, content));

            if (!result.isSuccess)
            {
                return Problem(result.errorMessage);
            }

            return Ok();
        }

        [HttpGet]
        [Route("{language}")]
        [SwaggerOperation(Summary = "Get content for a specific language")]
        [SwaggerResponse(200, "Content for specific language or all of them", typeof(ApplicationContentModel[]))]
        [SwaggerResponse(404, "No content found", typeof(void))]
        [SwaggerResponse(500, "Something went wrong when getting content", typeof(ProblemDetails))]
        public async Task<IActionResult> GetContentAsync(Language? language)
        {
            var result = await _mediator.Send(new GetApplicationContent(language));

            if (!result.isSuccess)
            {
                return Problem(result.errorMessage);
            }

            if (result.data == null || result.data.Length == 0)
            {
                return NotFound();
            }

            return Ok();
        }



    }
}
