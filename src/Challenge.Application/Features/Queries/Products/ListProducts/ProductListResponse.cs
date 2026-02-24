namespace Challenge.Application.Features.Queries.Products.ListProducts;

public record ProductResponse(Guid Id, string Description, decimal UnitPrice, int AvailableQuantity);
public record ProductListResponse(int Page, int PageSize, int TotalCount, List<ProductResponse> Products);
