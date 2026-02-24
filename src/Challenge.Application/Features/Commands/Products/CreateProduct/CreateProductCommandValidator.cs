using Challenge.Domain.Constants;
using FluentValidation;

namespace Challenge.Application.Features.Commands.Products.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(ProductConstants.MaxDescriptionLength)
            .WithMessage($"Description must not exceed {ProductConstants.MaxDescriptionLength} characters.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(ProductConstants.MinUnitPrice)
            .WithMessage($"Unit price must be at least {ProductConstants.MinUnitPrice}.");

        RuleFor(x => x.AvailableQuantity)
            .GreaterThanOrEqualTo(ProductConstants.MinAvailableQuantity)
            .WithMessage($"Available quantity must be at least {ProductConstants.MinAvailableQuantity}.");
    }
}