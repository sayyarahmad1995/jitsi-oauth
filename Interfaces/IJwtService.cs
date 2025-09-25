namespace jitsi_oauth.Interfaces;

public interface IJwtService
{
   string GenerateToken
   (
      string id,
      string username,
      string email,
      string Name,
      string room = "*",
      string picture = null,
      bool emailVerified = false
   );

   string JitsiDomain { get; }
}
