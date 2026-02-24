using Challenge.Application.Features.Shared.Products;
using Challenge.Domain.Entities;

namespace Challenge.Application.Features.Queries.Products.ListProducts;

internal static class ProductMappings
{
    public static ProductResponse ToDto(this Product product)
    {
        return new ProductResponse(product.Id, product.Description, product.UnitPrice, product.AvailableQuantity);
    }
}