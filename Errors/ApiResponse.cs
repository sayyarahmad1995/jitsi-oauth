namespace jitsi_oauth.Errors;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ApiResponse(int statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessageForStatusCode(statusCode);
    }

    private static string GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "You have made a bad request",
            401 => "You are not authorized to access this endpoint",
            404 => "The resource was not found",
            500 => "There occured an internal server error",
            _ => null
        };
    }
}
