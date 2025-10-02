using System.Text.Json;
using jitsi_oauth.Interfaces;
using Microsoft.Extensions.Logging;

namespace jitsi_oauth.Services;

public class KeycloakService : IKeycloakService
{
   private readonly HttpClient _httpClient;
   private readonly IConfiguration _config;
   private readonly ILogger<KeycloakService> _logger;
   private readonly string _baseUrl;
   private readonly string _realm;
   private readonly string _clientId;
   private readonly string _clientSecret;
   private readonly string _redirectUri;

   public KeycloakService(HttpClient httpClient, IConfiguration config, ILogger<KeycloakService> logger)
   {
      _httpClient = httpClient;
      _config = config;
      _logger = logger;

      _baseUrl = _config["KEYCLOAK_BASE_URL"];
      _realm = _config["KEYCLOAK_REALM"];
      _clientId = _config["KEYCLOAK_CLIENT_ID"];
      _clientSecret = _config["KEYCLOAK_CLIENT_SECRET"];
      _redirectUri = _config["KEYCLOAK_REDIRECT_URI"];
   }

   public string GetLoginUrl(string room = "*") =>
       $"{_baseUrl}/realms/{_realm}/protocol/openid-connect/auth?client_id={_clientId}&redirect_uri={Uri.EscapeDataString(_redirectUri)}&response_type=code&scope=openid&state={Uri.EscapeDataString(room)}";

   public async Task<JsonDocument> ExchangeCodeForTokenAsync(string code)
   {
      try
      {
         var tokenUrl = $"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token";

         var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
         {
            Content = new FormUrlEncodedContent(new[]
             {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUri)
                })
         };

         var response = await _httpClient.SendAsync(request);

         var responseContent = await response.Content.ReadAsStringAsync();

         if (!response.IsSuccessStatusCode)
         {
            _logger.LogError("Keycloak returned error: {StatusCode} - {Content}", response.StatusCode, responseContent);
         }

         response.EnsureSuccessStatusCode();

         return JsonDocument.Parse(responseContent);
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Failed to exchange code for token");
         throw;
      }
   }
}