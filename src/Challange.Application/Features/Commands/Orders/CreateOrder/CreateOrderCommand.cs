using Challange.Application.Commons;
using MediatR;

namespace Challange.Application.Features.Commands.Orders.CreateOrder;

public sealed record CreateOrderCommand(Guid CustomerId, string Currency, List<CreateOrderItemCommand> Items) : IRequest<Result<OrderResponse>>;
public record CreateOrderItemCommand(Guid ProductId, int Quantity);