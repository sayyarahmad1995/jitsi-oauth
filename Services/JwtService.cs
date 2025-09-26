using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using jitsi_oauth.Interfaces;

namespace jitsi_oauth.Services;

public class JwtService : IJwtService
{
   public string JitsiDomain { get; }
   private readonly IConfiguration _config;
   public JwtService(ILogger<JwtService> logger, IConfiguration config)
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
      string room = "*",
      string picture = null,
      bool emailVerified = false,
      bool moderator = false
   )
   {
      if (string.IsNullOrWhiteSpace(username))
         throw new ArgumentNullException(nameof(username));
      if (string.IsNullOrWhiteSpace(email))
         throw new ArgumentNullException(nameof(email));

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT_APP_SECRET"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var now = DateTimeOffset.UtcNow;

      var avatarUrl = !string.IsNullOrEmpty(picture)
         ? picture
         : $"https://robohash.org/{Uri.EscapeDataString(username)}";

      var userContext = new Dictionary<string, object>
      {
         ["id"] = id,
         ["name"] = Name,
         ["username"] = username,
         ["email"] = email,
         ["email_verified"] = emailVerified,
         ["avatar"] = avatarUrl,
         ["moderator"] = moderator,
      };

      var context = new Dictionary<string, object>
      {
         ["user"] = userContext
      };

      var payload = new JwtPayload
      {
         { "aud", _config["JWT_APP_ID"] },
         { "iss", _config["JWT_APP_ID"] },
         { "sub", id },
         { "room", room ?? "*" },
         { "nbf", now.ToUnixTimeSeconds() },
         { "iat", now.ToUnixTimeSeconds() },
         { "exp", now.AddDays(1).ToUnixTimeSeconds() },
         { "context", context }
      };

      var token = new JwtSecurityToken(new JwtHeader(creds), payload);
      var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

      return tokenString;
   }
}
