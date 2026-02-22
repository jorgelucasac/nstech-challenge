namespace Challange.Application.Commons;

public class Error
{
    public Error(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public Error(int code, string message, List<KeyValuePair<string, string>> errors)
    {
        Code = code;
        Message = message;
        Errors = errors;
    }

    public int Code { get; }
    public string Message { get; } = string.Empty;
    public List<KeyValuePair<string, string>> Errors { get; } = new List<KeyValuePair<string, string>>();
}