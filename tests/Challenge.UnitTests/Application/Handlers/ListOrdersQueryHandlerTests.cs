using Challenge.Application.Features.Queries.Orders.ListOrders;
using Challenge.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Challenge.Application.Contracts.Repositories;

namespace Challenge.UnitTests.Application.Handlers;

public class ListOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();

    private readonly ListOrdersQueryHandler _handler;

    public ListOrdersQueryHandlerTests()
    {
        _handler = new ListOrdersQueryHandler(
            _orderRepository.Object,
            NullLogger<ListOrdersQueryHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedOrders()
    {
        var order = new Order(Guid.NewGuid(), "usd");
        order.AddItems([new OrderItem(order.Id, Guid.NewGuid(), 5m, 2)]);

        _orderRepository
            .Setup(repo => repo.ListAsync(null, null, null, null, 1, 10, CancellationToken.None))
            .ReturnsAsync((Orders: (IReadOnlyList<Order>)new List<Order> { order }, TotalCount: 1));

        var result = await _handler.HandleAsync(new ListOrdersQuery(null, null, null, null, 1, 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
        result.Value.Orders.Should().HaveCount(1);
        result.Value.Orders[0].Id.Should().Be(order.Id);
    }
}
