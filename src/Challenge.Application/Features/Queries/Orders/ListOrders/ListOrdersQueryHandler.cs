using Challenge.Application.Commons;
using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Features;
using Challenge.Application.Features.Commands.Orders;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Features.Queries.Orders.ListOrders;

public sealed class ListOrdersQueryHandler(
    IOrderRepository orderRepository,
    ILogger<ListOrdersQueryHandler> logger) : BaseHandler<ListOrdersQuery, OrderListResponse>(logger)
{
    public override async Task<Result<OrderListResponse>> HandleAsync(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing orders page {Page} size {PageSize}", request.Page, request.PageSize);

        var (orders, totalCount) = await orderRepository.ListAsync(
            request.CustomerId,
            request.Status,
            request.From,
            request.To,
            request.Page,
            request.PageSize,
            cancellationToken);

        var response = new OrderListResponse(request.Page, request.PageSize, totalCount, [.. orders.Select(x => x.ToDto())]);
        return Result.Success(response);
    }
}