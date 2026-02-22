namespace Challange.Application.Commons;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public Error Error { get; }

    public static Result<T> Success<T>(T value) => new(true, value, null);

    public static Result<T> Failure<T>(Error error) => new(false, default!, error);

    public static Result<T> Failure<T>(int code, string message, List<KeyValuePair<string, string>> details) => new(false, default!, new Error(code, message, details));

    public static Result<T> Failure<T>(int code, string message) => new(false, default!, new Error(code, message));

    public static Result<T> NotFound<T>(string message) => Failure<T>(404, message);

    public static Result<T> Validation<T>(string message) => Failure<T>(400, message);

    public static Result<T> Validation<T>(string message, List<KeyValuePair<string, string>> details) => Failure<T>(422, message, details);

    public static Result<T> Unauthorized<T>(string message) => Failure<T>(401, message);
}

public class Result<T> : Result
{
    public T Value { get; }

    internal Result(bool isSuccess, T value, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Validation(string message, List<KeyValuePair<string, string>> details) => Failure<T>(422, message, details);
}