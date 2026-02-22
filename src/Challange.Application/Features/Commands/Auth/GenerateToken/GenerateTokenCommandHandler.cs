using Challange.Application.Commons;
using Challange.Application.Contracts.Repositories;
using Challange.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Challange.Application.Features.Commands.Auth.GenerateToken;

public sealed class GenerateTokenCommandHandler(
    IUserRepository userRepository,
    IPasswordHasherService passwordHasher,
    ITokenService tokenService,
    ILogger<GenerateTokenCommandHandler> logger) : BaseHandler<GenerateTokenCommand, GenerateTokenResponse>(logger)
{
    public override async Task<Result<GenerateTokenResponse>> HandleAsync(GenerateTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating token for login {Login}", request.Login);

        var user = await userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result.Unauthorized<GenerateTokenResponse>("Invalid credentials.");
        }

        var isValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isValid)
        {
            logger.LogWarning("GenerateToken failed due to invalid credentials for login {Login}", request.Login);
            return Result.Unauthorized<GenerateTokenResponse>("Invalid credentials.");
        }

        var token = tokenService.GenerateToken(user.Id, user.Login);
        var response = new GenerateTokenResponse(token.AccessToken, token.ExpiresAtUtc);

        return Result.Success(response);
    }
}