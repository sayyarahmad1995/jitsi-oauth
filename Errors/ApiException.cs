using System.Text.Json.Serialization;

namespace jitsi_oauth.Errors;

public class ApiException : ApiResponse
{
    public ApiException(int statusCode, string message = null, string details = null) : base(statusCode, message)
    {
        Details = details;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
   public string Details { get; set; }
    
}
