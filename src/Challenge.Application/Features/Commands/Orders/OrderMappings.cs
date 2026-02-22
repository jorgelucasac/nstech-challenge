using Challenge.Domain.Entities;

namespace Challenge.Application.Features.Commands.Orders;

internal static class OrderMappings
{
    public static OrderResponse ToDto(this Order order)
    {
        return new OrderResponse(
            order.Id,
            order.UserId,
            order.Status,
            order.Currency,
            order.Total,
            order.CreatedAt.LocalDateTime,
            order.Items.ConvertAll(i => new OrderItemResponse(i.ProductId, i.UnitPrice, i.Quantity, i.Subtotal)));
    }
}