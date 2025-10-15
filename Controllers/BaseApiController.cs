using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace jitsi_oauth.Controllers;

[ApiController]
[Route("api/v1/")]
public class BaseApiController : ControllerBase
{
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        var healthInfo = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        };

        return Ok(healthInfo);
    }
}
