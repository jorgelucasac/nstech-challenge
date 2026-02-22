namespace Challange.Application.Commons;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public Error Error { get; }

    private Result(bool isSuccess, T value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(Error error) => new(false, default!, error);

    public static Result<T> Failure(int code, string message, List<KeyValuePair<string, string>> errors) => new(false, default!, new Error(code, message, errors));

    public static Result<T> Failure(int code, string message) => new(false, default!, new Error(code, message));

    public static Result<T> NotFound(string message) => Failure(404, message);

    public static Result<T> BadRequest(string message) => Failure(400, message);

    public static Result<T> Validation(string message, List<KeyValuePair<string, string>> errors) => Failure(422, message, errors);

    public static Result<T> Unauthorized(string message) => Failure(401, message);
}