using Challange.Application.Features.Commands.Orders;

namespace Challange.Application.Features.Queries.Orders.ListOrders;

public record OrderListResponse(int Page, int PageSize, int TotalCount, List<OrderResponse> Orders);