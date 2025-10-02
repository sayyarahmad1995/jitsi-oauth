using System.Text;
using System.Text.Json;

namespace jitsi_oauth.Helpers;

public static class TokenHelper
{
   public static JsonDocument ParseIdToken(string idToken)
   {
      var payload = idToken.Split('.')[1];
      var json = Base64UrlDecode(payload);
      return JsonDocument.Parse(json);
   }

   private static string Base64UrlDecode(string input)
   {
      string base64 = input.Replace('-', '+').Replace('_', '/');
      switch (base64.Length % 4)
      {
         case 2: base64 += "=="; break;
         case 3: base64 += "="; break;
      }
      var bytes = Convert.FromBase64String(base64);
      return Encoding.UTF8.GetString(bytes);
   }
}
