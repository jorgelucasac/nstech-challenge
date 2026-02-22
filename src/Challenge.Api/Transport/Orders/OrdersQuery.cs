using Challenge.Application.Features.Queries.Orders.ListOrders;
using Challenge.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Transport.Orders;

public class OrdersQuery
{
    [FromQuery]
    public Guid? CustomerId { get; set; }

    [FromQuery]
    public OrderStatus? Status { get; set; }

    [FromQuery]
    public DateTime? From { get; set; }

    [FromQuery]
    public DateTime? To { get; set; }

    [FromQuery]
    public int Page { get; set; } = 1;

    [FromQuery]
    public int PageSize { get; set; } = 10;

    public ListOrdersQuery ToQuery()
    {
        return new ListOrdersQuery(CustomerId, Status, From, To, Page, PageSize);
    }
}