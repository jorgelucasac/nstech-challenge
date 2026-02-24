using Challenge.Application.Features.Shared.Products;

namespace Challenge.Application.Features.Queries.Products.ListProducts;
public record ProductListResponse(int Page, int PageSize, int TotalCount, List<ProductResponse> Products);