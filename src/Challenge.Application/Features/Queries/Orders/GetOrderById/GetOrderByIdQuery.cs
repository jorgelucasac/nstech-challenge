using Challenge.Application.Commons;
using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Features;
using Challenge.Application.Features.Commands.Orders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Features.Queries.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderResponse>>;

public sealed class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    ILogger<GetOrderByIdQueryHandler> logger)
    : BaseHandler<GetOrderByIdQuery, OrderResponse>(logger)
{
    public override async Task<Result<OrderResponse>> HandleAsync(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdReadOnlyWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            logger.LogWarning("GetOrderById failed: order with ID {OrderId} not found", request.OrderId);
            return Result.NotFound<OrderResponse>("Order not found.");
        }
        return Result.Success(order.ToDto());
    }
}