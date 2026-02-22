namespace Challenge.Application.Features.Commands.Auth.GenerateToken;

public sealed record GenerateTokenResponse(string Token, DateTime ExpiresAtUtc);