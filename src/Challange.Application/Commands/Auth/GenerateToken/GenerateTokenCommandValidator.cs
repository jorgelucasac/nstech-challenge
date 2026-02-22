using Challange.Domain.Constants;
using FluentValidation;

namespace Challange.Application.Commands.Auth.GenerateToken;

public sealed class GenerateTokenCommandValidator : AbstractValidator<GenerateTokenCommand>
{
    public GenerateTokenCommandValidator()
    {
        RuleFor(x => x.Login)
            .MinimumLength(UserConstants.MinLoginLength).WithMessage($"Login must be at least {UserConstants.MinLoginLength} characters long.")
            .MaximumLength(UserConstants.MaxLoginLength).WithMessage($"Login must not exceed {UserConstants.MaxLoginLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(UserConstants.MinPasswordLength).WithMessage($"Password must be at least {UserConstants.MinPasswordLength} characters long.");
    }
}