using Challange.Application.Features.Commands.Orders.CreateOrder;

namespace Challange.Api.Transport.Orders;

public record CreateOrderRequest(Guid CustomerId, string Currency, List<CreateOrderItemRequest> Items)
{
    public CreateOrderCommand ToCommand() => new(CustomerId, Currency, Items.Select(i => i.ToCommand()).ToList());
}
public record CreateOrderItemRequest(Guid ProductId, int Quantity)
{
    public CreateOrderItemCommand ToCommand() => new(ProductId, Quantity);
}