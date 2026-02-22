using Challenge.Application.Commons;
using Challenge.Application.Features.Commands.Orders;
using MediatR;

namespace Challenge.Application.Features.Commands.Orders.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId) : IRequest<Result<OrderResponse>>;