using Challenge.Application.Commons;
using Challenge.Application.Contracts.Repositories;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Features.Queries.Products.ListProducts;

public sealed class ListProductsQueryHandler(
    IProductRepository productRepository,
    ILogger<ListProductsQueryHandler> logger) : BaseHandler<ListProductsQuery, ProductListResponse>(logger)
{
    public override async Task<Result<ProductListResponse>> HandleAsync(ListProductsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing products {@Query}", request);

        var (products, totalCount) = await productRepository.ListAsync(request.Name, request.Page, request.PageSize, cancellationToken);
        var response = new ProductListResponse(request.Page, request.PageSize, totalCount, [.. products.Select(p => p.ToDto())]);
        return Result.Success(response);
    }
}