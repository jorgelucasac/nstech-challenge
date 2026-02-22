using Challenge.Application.Commons;
using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Contracts.Services;
using Challenge.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Features.Commands.Auth.RegisterUser;

public class RegisterUserCommandHandler(

    IUserRepository userRepository,
    IPasswordHasherService passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<RegisterUserCommandHandler> logger)
    : BaseHandler<RegisterUserCommand, RegisterUserResponse>(logger)
{
    public override async Task<Result<RegisterUserResponse>> HandleAsync(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering user {Login}", request.Login);

        var alreadyExists = await userRepository.ExistsByLoginAsync(request.Login, cancellationToken);
        if (alreadyExists)
        {
            logger.LogWarning("User with login {Login} already exists", request.Login);
            return Result.Validation<RegisterUserResponse>("User with this login already exists");
        }

        var hash = passwordHasher.HashPassword(request.Password);
        var user = new User(request.Login, hash);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("User {Login} registered with id {UserId}", user.Login, user.Id);
        return Result.Success(new RegisterUserResponse(user.Id, user.Login));
    }
}