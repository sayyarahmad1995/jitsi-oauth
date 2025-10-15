using jitsi_oauth.DTOs;
using jitsi_oauth.Errors;
using jitsi_oauth.Helpers;
using jitsi_oauth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace jitsi_oauth.Controllers;

public class AuthController : BaseApiController
{
   private readonly IKeycloakService _keycloak;
   private readonly IJwtService _jwt;
   private readonly IClaimMapper _mapper;
   private readonly IConfiguration _config;
   public AuthController(IKeycloakService keycloak, IJwtService jwt, IClaimMapper mapper, IConfiguration config)
   {
      _config = config;
      _mapper = mapper;
      _keycloak = keycloak;
      _jwt = jwt;
   }

   [HttpGet("login")]
   public ActionResult Login([FromQuery] string room = "*")
   {
      return Redirect(_keycloak.GetLoginUrl(room));
   }

   [HttpGet("callback")]
   public async Task<ActionResult> Callback([FromQuery] string code, [FromQuery] string state)
   {
      if (string.IsNullOrEmpty(code))
         return NotFound(new ApiResponse(404));

      var tokenJson = await _keycloak.ExchangeCodeForTokenAsync(code);
      var idToken = tokenJson.RootElement.GetProperty("id_token").GetString();
      var userPayload = TokenHelper.ParseIdToken(idToken);

      var claims = _mapper.Map<KeycloakUserClaimsDTO>(userPayload);

      var room = string.IsNullOrEmpty(state) ? "*" : state;

      var jitsiToken = _jwt.GenerateToken(
         claims.sub,
         claims.preferred_username,
         claims.email,
         claims.Name,
         claims.email_verified
      );

      var redirectUrl = $"https://{_jwt.JitsiDomain}/{room}?jwt={jitsiToken}";

      var configParams = new List<string>();

      bool disablePrejoinPage = _config["DISABLE_PREJOIN_PAGE"] == "true";
      if (disablePrejoinPage)
         configParams.Add("config.prejoinConfig.enabled=false");

      if (configParams.Count > 0)
         redirectUrl += "#" + string.Join("&", configParams);

      return Redirect(redirectUrl);
   }
}