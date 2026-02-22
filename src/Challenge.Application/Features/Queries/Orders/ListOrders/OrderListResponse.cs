using Challenge.Application.Features.Commands.Orders;

namespace Challenge.Application.Features.Queries.Orders.ListOrders;

public record OrderListResponse(int Page, int PageSize, int TotalCount, List<OrderResponse> Orders);