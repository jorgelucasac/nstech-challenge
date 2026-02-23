namespace Challenge.Application.Dtos;

public record TokenResponse(string AccessToken, DateTime ExpiresAtUtc);