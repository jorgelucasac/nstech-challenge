using Challenge.Api.Transport;
using Challenge.Api.Transport.Products;
using Challenge.Application.Features.Queries.Products.ListProducts;
using Challenge.Application.Features.Shared.Products;
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

    [HttpPost]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        return ErrorResponse(result.Error);
    }
}