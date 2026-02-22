using Challange.Application.Commons;
using Challange.Application.Contracts.Repositories;
using Challange.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Challange.Application.Features.Commands.Orders.ConfirmOrder;

public sealed class ConfirmOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<ConfirmOrderCommandHandler> logger) : BaseHandler<ConfirmOrderCommand, OrderResponse>(logger)
{
    public override async Task<Result<OrderResponse>> HandleAsync(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Confirming order {OrderId}", request.OrderId);

        var order = await orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("ConfirmOrder failed: order {OrderId} not found", request.OrderId);
            return Result.NotFound<OrderResponse>("Order not found.");
        }

        if (order.Status == OrderStatus.Confirmed)
        {
            logger.LogInformation("Order {OrderId} already confirmed (idempotent)", request.OrderId);
            return Result.Success(order.ToDto());
        }

        if (order.Status != OrderStatus.Placed)
        {
            logger.LogWarning("ConfirmOrder failed: order {OrderId} in invalid status {Status}", request.OrderId, order.Status);
            return Result.Validation<OrderResponse>("Only orders in Placed status can be confirmed.");
        }

        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

        var errorDetails = new List<KeyValuePair<string, string>>();

        foreach (var item in order.Items)
        {
            if (products[item.ProductId].AvailableQuantity < item.Quantity)
            {
                logger.LogWarning("ConfirmOrder failed: insufficient stock for product {ProductId}", item.ProductId);
                errorDetails.Add(new(item.ProductId.ToString(), "Insufficient stock"));
            }
        }

        if (errorDetails.Count > 0)
        {
            return Result.Validation<OrderResponse>("Insufficient stock for one or more products.", errorDetails);
        }

        foreach (var item in order.Items)
        {
            products[item.ProductId].DecreaseStock(item.Quantity);
        }

        if (!order.TryConfirm())
        {
            logger.LogWarning("ConfirmOrder failed: invalid transition for order {OrderId}", request.OrderId);
            return Result.Validation<OrderResponse>("Invalid state transition for confirmation.");
        }

        await unitOfWork.CommitAsync(cancellationToken);
        logger.LogInformation("Order {OrderId} confirmed successfully", request.OrderId);
        return Result.Success(order.ToDto());
    }
}