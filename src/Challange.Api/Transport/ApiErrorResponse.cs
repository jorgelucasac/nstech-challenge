using Challange.Application.Commons;

namespace Challange.Api.Transport;

public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public List<KeyValuePair<string, string>> Errors { get; set; } = [];

    public static ApiErrorResponse FromError(Error error)
    {
        return new ApiErrorResponse
        {
            Message = error.Message,
            Errors = error.Details
        };
    }
}