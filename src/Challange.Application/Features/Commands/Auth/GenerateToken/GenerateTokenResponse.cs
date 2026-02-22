namespace Challange.Application.Features.Commands.Auth.GenerateToken;

public sealed record GenerateTokenResponse(string Token, DateTime ExpiresAtUtc);