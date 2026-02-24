using Challenge.Api.Transport.Products;
using Challenge.Application.Features.Queries.Products.ListProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers;

[Route("api/v{version:apiVersion}/products")]
[Authorize]
public class ProductsController(IMediator mediator) : ControllerMain
{
    [HttpGet]
    [ProducesResponseType<ProductListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync([FromQuery] ProductsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query.ToQuery(), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return ErrorResponse(result.Error);
    }
}
