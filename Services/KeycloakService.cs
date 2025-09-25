using System.Text.Json;
using jitsi_oauth.Interfaces;

namespace jitsi_oauth.Services;

public class KeycloakService : IKeycloakService
{
   private readonly HttpClient _httpClient;
   private readonly IConfiguration _config;
   private readonly string _baseUrl;
   private readonly string _realm;
   private readonly string _clientId;
   private readonly string _clientSecret;
   private readonly string _redirectUri;

   public KeycloakService(HttpClient httpClient, IConfiguration config)
   {
      _httpClient = httpClient;
      _config = config;
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
      var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token")
      {
         Content = new FormUrlEncodedContent(new[]
         {
            new KeyValuePair<string,string>("grant_type","authorization_code"),
            new KeyValuePair<string,string>("code",code),
            new KeyValuePair<string,string>("client_id",_clientId),
            new KeyValuePair<string,string>("client_secret",_clientSecret),
            new KeyValuePair<string,string>("redirect_uri",_redirectUri)
         })
      };

      var response = await _httpClient.SendAsync(request);
      var json = await response.Content.ReadAsStringAsync();
      response.EnsureSuccessStatusCode();

      return JsonDocument.Parse(json);
   }
}
