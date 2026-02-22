using Challenge.Domain.Enums;

namespace Challenge.Application.Features.Commands.Orders;

public record OrderResponse(Guid Id, Guid CustomerId, OrderStatus Status, string Currency, decimal Total, DateTime CreatedAt, List<OrderItemResponse> Items);
public record OrderItemResponse(Guid ProductId, decimal UnitPrice, int Quantity, decimal Subtotal);