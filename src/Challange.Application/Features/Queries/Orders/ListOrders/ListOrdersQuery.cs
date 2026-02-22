using Challange.Application.Commons;
using Challange.Domain.Enums;
using MediatR;

namespace Challange.Application.Features.Queries.Orders.ListOrders;

public sealed record ListOrdersQuery(
    Guid? CustomerId,
    OrderStatus? Status,
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<OrderListResponse>>;