using Challenge.Application.Features.Commands.Orders.CancelOrder;
using Challenge.Domain.Entities;
using Challenge.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Challenge.Application.Contracts.Repositories;
using System.Reflection;

namespace Challenge.UnitTests.Application.Handlers;

public class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _handler = new CancelOrderCommandHandler(
            _orderRepository.Object,
            _productRepository.Object,
            _unitOfWork.Object,
            NullLogger<CancelOrderCommandHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidation_WhenOrderMissing()
    {
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((Order?)null);
        var result = await _handler.HandleAsync(new CancelOrderCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(400);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAlreadyCanceled()
    {
        var order = CreateOrderWithItem();
        order.TryCancel();
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, CancellationToken.None))
            .ReturnsAsync(order);

        var result = await _handler.HandleAsync(new CancelOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(OrderStatus.Canceled);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidation_WhenStatusInvalid()
    {
        var order = CreateOrderWithItem();
        SetStatus(order, OrderStatus.Draft);
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, CancellationToken.None))
            .ReturnsAsync(order);

        var result = await _handler.HandleAsync(new CancelOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(400);
    }

    [Fact]
    public async Task HandleAsync_ShouldCancelOrder_WhenPlaced()
    {
        var order = CreateOrderWithItem();
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, CancellationToken.None))
            .ReturnsAsync(order);
        _unitOfWork.Setup(work => work.CommitAsync(CancellationToken.None)).ReturnsAsync(1);

        var result = await _handler.HandleAsync(new CancelOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(OrderStatus.Canceled);
        _unitOfWork.Verify(work => work.CommitAsync(CancellationToken.None), Times.Once);
    }

    private static Order CreateOrderWithItem()
    {
        var order = new Order(Guid.NewGuid(), "usd");
        var item = new OrderItem(order.Id, Guid.NewGuid(), 10m, 1);
        order.AddItems([item]);
        return order;
    }

    private static void SetStatus(Order order, OrderStatus status)
    {
        var property = typeof(Order).GetProperty("Status", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        property!.SetValue(order, status);
    }
}
