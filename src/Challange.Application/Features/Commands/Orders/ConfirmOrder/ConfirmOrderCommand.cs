using Challange.Application.Commons;
using MediatR;

namespace Challange.Application.Features.Commands.Orders.ConfirmOrder;

public sealed record ConfirmOrderCommand(Guid OrderId) : IRequest<Result<OrderResponse>>;