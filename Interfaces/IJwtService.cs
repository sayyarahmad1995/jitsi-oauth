namespace jitsi_oauth.Interfaces;

public interface IJwtService
{
   string GenerateToken
   (
      string id,
      string username,
      string email,
      string Name,
      bool emailVerified = false
   );

   string JitsiDomain { get; }
}
