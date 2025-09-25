using jitsi_oauth.Errors;
using jitsi_oauth.Interfaces;
using jitsi_oauth.Services;
using Microsoft.AspNetCore.Mvc;

namespace jitsi_oauth.Extensions;

public static class AppServiceExtension
{
   public static IServiceCollection AddAppServices(this IServiceCollection services)
   {
      services.AddHttpClient<IKeycloakService, KeycloakService>();
      services.AddSingleton<IJwtService, JwtService>();
      services.AddSingleton<IClaimMapper, ClaimMapper>();
      services.AddLogging();

      services.Configure<ApiBehaviorOptions>(opt =>
      {
         opt.InvalidModelStateResponseFactory = ActionContext =>
         {
            var errors = ActionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(e => e.Value.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            var errorResponse = new ApiValidationErrorResponse { Errors = errors };

            return new BadRequestObjectResult(errorResponse);
         };
      });

      return services;
   }
}
