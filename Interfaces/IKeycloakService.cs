using System.Text.Json;

namespace jitsi_oauth.Interfaces;

public interface IKeycloakService
{
   string GetLoginUrl(string room = "*");
   
   Task<JsonDocument> ExchangeCodeForTokenAsync(string code);
}
