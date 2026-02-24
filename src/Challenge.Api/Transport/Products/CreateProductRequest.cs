using Challenge.Application.Features.Commands.Products.CreateProduct;

namespace Challenge.Api.Transport.Products;

public record CreateProductRequest(string Description, decimal UnitPrice, int AvailableQuantity)
{
    public CreateProductCommand ToCommand() => new(Description, UnitPrice, AvailableQuantity);
}
