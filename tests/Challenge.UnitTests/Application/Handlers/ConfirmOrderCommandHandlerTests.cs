using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Features.Commands.Orders.ConfirmOrder;
using Challenge.Domain.Entities;
using Challenge.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Reflection;

namespace Challenge.UnitTests.Application.Handlers;

public class ConfirmOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly ConfirmOrderCommandHandler _handler;

    public ConfirmOrderCommandHandlerTests()
    {
        _handler = new ConfirmOrderCommandHandler(
            _orderRepository.Object,
            _productRepository.Object,
            _unitOfWork.Object,
            NullLogger<ConfirmOrderCommandHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenOrderMissing()
    {
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((Order?)null);

        var result = await _handler.HandleAsync(new ConfirmOrderCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(404);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAlreadyConfirmed()
    {
        var order = CreateOrderWithItem();
        order.TryConfirm();
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, CancellationToken.None))
            .ReturnsAsync(order);

        var result = await _handler.HandleAsync(new ConfirmOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidation_WhenStatusInvalid()
    {
        var order = CreateOrderWithItem();
        SetStatus(order, OrderStatus.Draft);
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, CancellationToken.None))
            .ReturnsAsync(order);

        var result = await _handler.HandleAsync(new ConfirmOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(400);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidation_WhenInsufficientStock()
    {
        var order = CreateOrderWithItem();
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, CancellationToken.None))
            .ReturnsAsync(order);

        var product = new Product("Item", 10m, 1);
        product.DecreaseStock(1);

        _productRepository
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ReturnsAsync(new Dictionary<Guid, Product>
            {
                [order.Items[0].ProductId] = product
            });

        var result = await _handler.HandleAsync(new ConfirmOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(422);
    }

    [Fact]
    public async Task HandleAsync_ShouldConfirmOrder_WhenStockAvailable()
    {
        var order = CreateOrderWithItem();
        var product = new Product("Item", 10m, 5);
        _orderRepository
            .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, CancellationToken.None))
            .ReturnsAsync(order);

        _productRepository
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ReturnsAsync(new Dictionary<Guid, Product>
            {
                [order.Items[0].ProductId] = product
            });

        _unitOfWork.Setup(work => work.CommitAsync(CancellationToken.None)).ReturnsAsync(1);

        var result = await _handler.HandleAsync(new ConfirmOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(OrderStatus.Confirmed);
        product.AvailableQuantity.Should().Be(4);
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