namespace Challange.Application.Commons;

public class Error
{
    public Error(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public Error(int code, string message, List<KeyValuePair<string, string>> details)
    {
        Code = code;
        Message = message;
        Details = details;
    }

    public int Code { get; }
    public string Message { get; } = string.Empty;
    public List<KeyValuePair<string, string>> Details { get; } = new List<KeyValuePair<string, string>>();
}