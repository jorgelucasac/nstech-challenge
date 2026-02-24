using Challenge.Application.Features.Queries.Products.ListProducts;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Transport.Products;

public class ProductsQuery
{
    [FromQuery]
    public string? Name { get; set; }

    [FromQuery]
    public int Page { get; set; } = 1;

    [FromQuery]
    public int PageSize { get; set; } = 10;

    public ListProductsQuery ToQuery()
    {
        return new ListProductsQuery(Name, Page, PageSize);
    }
}
