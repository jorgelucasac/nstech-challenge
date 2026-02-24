using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Features.Commands.Orders.CreateOrder;
using Challenge.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Challenge.UnitTests.Application.Handlers;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _handler = new CreateOrderCommandHandler(
            _orderRepository.Object,
            _productRepository.Object,
            _userRepository.Object,
            _unitOfWork.Object,
            NullLogger<CreateOrderCommandHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidation_WhenUserNotFound()
    {
        var request = new CreateOrderCommand(Guid.NewGuid(), "usd", [new CreateOrderItemCommand(Guid.NewGuid(), 1)]);

        _userRepository
            .Setup(x => x.ExistsByIdAsync(request.CustomerId, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _handler.HandleAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(400);
        result.Error.Message.Should().Be("Customer not found.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidation_WhenProductMissing()
    {
        var request = new CreateOrderCommand(Guid.NewGuid(), "usd", [new CreateOrderItemCommand(Guid.NewGuid(), 1)]);
        _productRepository
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ReturnsAsync(new Dictionary<Guid, Product>());
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(400);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidation_WhenInsufficientStock()
    {
        var productId = Guid.NewGuid();
        var request = new CreateOrderCommand(Guid.NewGuid(), "usd", [new CreateOrderItemCommand(productId, 2)]);

        _userRepository
            .Setup(x => x.ExistsByIdAsync(request.CustomerId, CancellationToken.None))
            .ReturnsAsync(true);

        _productRepository
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ReturnsAsync(new Dictionary<Guid, Product>
            {
                [productId] = new Product("Item", 10m, 1)
            });

        var result = await _handler.HandleAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(422);
        result.Error.Details.Should().ContainSingle(detail => detail.Key == productId.ToString());
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateOrder_WhenStockAvailable()
    {
        var productId = Guid.NewGuid();
        var request = new CreateOrderCommand(Guid.NewGuid(), "usd", [new CreateOrderItemCommand(productId, 2)]);

        _userRepository
            .Setup(x => x.ExistsByIdAsync(request.CustomerId, CancellationToken.None))
            .ReturnsAsync(true);

        _productRepository
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ReturnsAsync(new Dictionary<Guid, Product>
            {
                [productId] = new Product("Item", 10m, 5)
            });
        _orderRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Order>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(work => work.CommitAsync(CancellationToken.None)).ReturnsAsync(1);

        var result = await _handler.HandleAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Currency.Should().Be("USD");
        result.Value.Total.Should().Be(20m);
        result.Value.Items.Should().HaveCount(1);
        _orderRepository.Verify(repo => repo.AddAsync(It.IsAny<Order>(), CancellationToken.None), Times.Once);
        _unitOfWork.Verify(work => work.CommitAsync(CancellationToken.None), Times.Once);
    }
}