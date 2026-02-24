using Challenge.Application.Commons;
using Challenge.Application.Features.Shared.Products;
using MediatR;

namespace Challenge.Application.Features.Commands.Products.CreateProduct;

public sealed record CreateProductCommand(string Description, decimal UnitPrice, int AvailableQuantity) : IRequest<Result<ProductResponse>>;