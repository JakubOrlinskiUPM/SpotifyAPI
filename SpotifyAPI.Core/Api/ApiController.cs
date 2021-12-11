using Microsoft.AspNetCore.Mvc;

namespace SpotifyAPI.Core.Api
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(void), 401)]
    [ProducesResponseType(typeof(void), 403)]
    [ProducesResponseType(typeof(void), 404)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public abstract class ApiController : ControllerBase { }
}