using Challange.Application.Commons;
using Challange.Application.Contracts.Repositories;
using Challange.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Challange.Application.Features.Commands.Orders.CancelOrder;

public sealed class CancelOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<CancelOrderCommandHandler> logger) : IRequestHandler<CancelOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Canceling order {OrderId}", request.OrderId);

        var order = await orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("CancelOrder failed: order {OrderId} not found", request.OrderId);
            return Result.Validation<OrderResponse>("Order not found.");
        }

        if (order.Status == OrderStatus.Canceled)
        {
            logger.LogInformation("Order {OrderId} already canceled (idempotent)", request.OrderId);
            return Result.Success(order.ToDto());
        }

        if (order.Status is not (OrderStatus.Placed or OrderStatus.Confirmed))
        {
            logger.LogWarning("CancelOrder failed: order {OrderId} in invalid status {Status}", request.OrderId, order.Status);
            return Result.Validation<OrderResponse>("Only orders in Placed or Confirmed status can be canceled.");
        }

        if (!order.TryCancel())
        {
            logger.LogWarning("CancelOrder failed: invalid transition for order {OrderId}", request.OrderId);
            return Result.Validation<OrderResponse>("Invalid state transition for cancellation.");
        }

        if (order.Status == OrderStatus.Confirmed)
        {
            var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

            foreach (var item in order.Items)
            {
                products[item.ProductId].IncreaseStock(item.Quantity);
            }
        }

        await unitOfWork.CommitAsync(cancellationToken);
        logger.LogInformation("Order {OrderId} canceled successfully", request.OrderId);
        return Result.Success(order.ToDto());
    }
}