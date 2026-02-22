using Challange.Domain.Constants;
using FluentValidation;

namespace Challange.Application.Features.Commands.Auth.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage("Login is required.")
            .MinimumLength(UserConstants.MinLoginLength)
            .WithMessage($"Login must be at least {UserConstants.MinLoginLength} characters long.")
            .MaximumLength(UserConstants.MaxLoginLength)
            .WithMessage($"Login must be at most {UserConstants.MaxLoginLength} characters long.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(UserConstants.MinPasswordLength)
            .WithMessage($"Password must be at least {UserConstants.MinPasswordLength} characters long.");
    }
}