using jitsi_oauth.Errors;
using Microsoft.AspNetCore.Mvc;

namespace jitsi_oauth.Controllers;

[Route("errors/{code}")]
public class ErrorController : BaseApiController
{
   public IActionResult Error(int code)
   {
      return new ObjectResult(new ApiResponse(code));
   }
}
