using Challange.Api.Transport;
using Challange.Api.Transport.Orders;
using Challange.Application.Features.Commands.Orders;
using Challange.Application.Features.Commands.Orders.CancelOrder;
using Challange.Application.Features.Commands.Orders.ConfirmOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Challange.Api.Controllers;

[Route("api/v{version:apiVersion}/orders")]
[Authorize]
public class OrdersController(IMediator mediator) : ControllerMain
{
    [HttpPost]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);

        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        return ErrorResponse(result.Error);
    }

    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ConfirmAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ConfirmOrderCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return ErrorResponse(result.Error);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CancelAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CancelOrderCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return ErrorResponse(result.Error);
    }
}