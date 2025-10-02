namespace jitsi_oauth.DTOs;

public class KeycloakUserClaimsDTO
{
   public string sub { get; set; }
   public string preferred_username { get; set; }
   public string email { get; set; }
   public string picture { get; set; }
   public bool email_verified { get; set; }
   public string given_name { get; set; }
   public string family_name { get; set; }
   public string Name => $"{given_name} {family_name}".Trim();
   public bool moderator { get; set; }
}
