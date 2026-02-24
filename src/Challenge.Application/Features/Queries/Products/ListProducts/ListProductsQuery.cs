using Challenge.Application.Commons;
using MediatR;

namespace Challenge.Application.Features.Queries.Products.ListProducts;

public sealed record ListProductsQuery(string? Name, int Page = 1, int PageSize = 10) : IRequest<Result<ProductListResponse>>;
