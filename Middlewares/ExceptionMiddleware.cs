using System.Net;
using System.Text.Json;
using jitsi_oauth.Errors;

namespace jitsi_oauth.Middlewares;

public class ExceptionMiddleware
{
   private static readonly JsonSerializerOptions _jsonOptions = new()
   {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
   };
   private readonly RequestDelegate _next;
   private readonly ILogger<ExceptionMiddleware> _logger;
   private readonly IHostEnvironment _env;
   public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
   {
      _env = env;
      _logger = logger;
      _next = next;
   }

   public async Task InvokeAsync(HttpContext context)
   {
      try
      {
         await _next(context);
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

         context.Response.ContentType = "application/json";
         context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

         var response = _env.IsDevelopment()
         ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace)
         : new ApiException((int)HttpStatusCode.InternalServerError);

         var json = JsonSerializer.Serialize(response, _jsonOptions);

         await context.Response.WriteAsync(json);
      }
   }
}
