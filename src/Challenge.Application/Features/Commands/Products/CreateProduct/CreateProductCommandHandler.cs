using Challenge.Application.Commons;
using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Features;
using Challenge.Application.Features.Shared.Products;
using Challenge.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Features.Commands.Products.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateProductCommandHandler> logger) : BaseHandler<CreateProductCommand, ProductResponse>(logger)
{
    public override async Task<Result<ProductResponse>> HandleAsync(CreateProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product {Description}", request.Description);

        var product = new Product(request.Description, request.UnitPrice, request.AvailableQuantity);
        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        var response = new ProductResponse(product.Id, product.Description, product.UnitPrice, product.AvailableQuantity);
        return Result.Success(response);
    }
}