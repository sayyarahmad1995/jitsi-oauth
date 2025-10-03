using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using jitsi_oauth.Interfaces;

namespace jitsi_oauth.Services;

public class JwtService : IJwtService
{
   public string JitsiDomain { get; }
   private readonly IConfiguration _config;
   public JwtService(IConfiguration config)
   {
      _config = config;
      JitsiDomain = _config["JITSI_DOMAIN"];
   }

   public string GenerateToken
   (
      string id,
      string username,
      string email,
      string Name,
      bool emailVerified = false
   )
   {
      if (string.IsNullOrWhiteSpace(username))
         throw new ArgumentNullException(nameof(username));
      if (string.IsNullOrWhiteSpace(email))
         throw new ArgumentNullException(nameof(email));

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT_APP_SECRET"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var now = DateTimeOffset.UtcNow;

      var userContext = new Dictionary<string, object>
      {
         ["name"] = Name,
         ["username"] = username,
         ["email"] = email,
         ["email_verified"] = emailVerified,
         ["affiliation"] = "owner",
         ["moderator"] = true,
         ["lobby_bypass"] = true,
         ["security_bypass"] = true
      };

      var featuresContext = new Dictionary<string, object>
      {
         ["livestreaming"] = true,
         ["recording"] = true,
         ["screen-sharing"] = true
      };

      var context = new Dictionary<string, object>
      {
         ["user"] = userContext,
         ["features"] = featuresContext
      };

      var payload = new JwtPayload
      {
         { "aud", _config["JWT_APP_ID"] },
         { "iss", _config["JWT_APP_ID"] },
         { "iat", now.ToUnixTimeSeconds() },
         { "exp", now.AddDays(1).ToUnixTimeSeconds() },
         { "nbf", now.ToUnixTimeSeconds() },
         { "sub", id },
         { "context", context },
         { "room", "*" }
      };

      var token = new JwtSecurityToken(new JwtHeader(creds), payload);
      var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

      return tokenString;
   }
}
