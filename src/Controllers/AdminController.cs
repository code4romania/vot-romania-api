using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace VotRomania.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
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


    }
}