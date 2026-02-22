using Challange.Application.Commons;
using Challange.Application.Contracts.Repositories;
using Challange.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Challange.Application.Features.Commands.Orders.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateOrderCommandHandler> logger) : BaseHandler<CreateOrderCommand, OrderResponse>(logger)
{
    public override async Task<Result<OrderResponse>> HandleAsync(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items", request.CustomerId, request.Items.Count);

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

        if (products.Count != productIds.Count)
        {
            logger.LogWarning("CreateOrder failed: one or more products not found");
            return Result.Validation<OrderResponse>("One or more products not found.");
        }

        var errorDetails = new List<KeyValuePair<string, string>>();
        foreach (var item in request.Items)
        {
            if (products[item.ProductId].AvailableQuantity < item.Quantity)
            {
                logger.LogWarning("CreateOrder failed: insufficient stock for product {ProductId}", item.ProductId);
                errorDetails.Add(new KeyValuePair<string, string>(item.ProductId.ToString(), "Insufficient stock"));
            }
        }

        if (errorDetails.Count > 0)
        {
            return Result.Validation<OrderResponse>("Insufficient stock for one or more products.", errorDetails);
        }

        var order = new Order(request.CustomerId, request.Currency);
        var orderItems = request.Items.Select(i => new OrderItem(i.ProductId, order.Id, products[i.ProductId].UnitPrice, i.Quantity));
        order.AddItems(orderItems);

        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} created successfully", order.Id);
        var itemsResponse = order.Items.Select(i => new OrderItemResponse(i.ProductId, i.UnitPrice, i.Quantity, i.Subtotal)).ToList();
        return Result.Success(new OrderResponse(order.Id, order.UserId, order.Status, order.Currency, order.Total, order.CreatedAt.LocalDateTime, itemsResponse));
    }
}