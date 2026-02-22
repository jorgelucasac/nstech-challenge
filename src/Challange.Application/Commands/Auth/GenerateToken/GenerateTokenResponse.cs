namespace Challange.Application.Commands.Auth.GenerateToken;

public sealed record GenerateTokenResponse(string Token, DateTime ExpiresAtUtc);