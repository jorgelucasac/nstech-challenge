namespace Challange.Application.Dtos;

public record RegisterUserRequest(string Login, string Password);
public record TokenRequest(string Login, string Password);
public record TokenResponse(string AccessToken, DateTime ExpiresAtUtc);