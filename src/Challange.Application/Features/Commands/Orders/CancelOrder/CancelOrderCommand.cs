using Challange.Application.Commons;
using MediatR;

namespace Challange.Application.Features.Commands.Orders.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId) : IRequest<Result<OrderResponse>>;