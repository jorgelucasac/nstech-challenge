using Challenge.Application.Commons;
using Challenge.Application.Features.Commands.Orders;
using MediatR;

namespace Challenge.Application.Features.Commands.Orders.ConfirmOrder;

public sealed record ConfirmOrderCommand(Guid OrderId) : IRequest<Result<OrderResponse>>;