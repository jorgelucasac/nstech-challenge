using Challenge.Domain.Constants;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using FluentAssertions;

namespace Challenge.UnitTests.Domain;

public class OrderItemTests
{
    [Fact]
    public void Constructor_ShouldInitializeOrderItem()
    {
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var item = new OrderItem(orderId, productId, 15m, 2);

        item.OrderId.Should().Be(orderId);
        item.ProductId.Should().Be(productId);
        item.UnitPrice.Should().Be(15m);
        item.Quantity.Should().Be(2);
        item.Subtotal.Should().Be(30m);
    }

    [Fact]
    public void Constructor_ShouldThrowWhenValuesAreInvalid()
    {
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        Action emptyOrderId = () => new OrderItem(Guid.Empty, productId, 10m, 1);
        Action emptyProductId = () => new OrderItem(orderId, Guid.Empty, 10m, 1);
        Action invalidPrice = () => new OrderItem(orderId, productId, OrderItemConstants.MinUnitPrice - 0.01m, 1);
        Action invalidQuantity = () => new OrderItem(orderId, productId, 10m, OrderItemConstants.MinQuantity - 1);

        emptyOrderId.Should().Throw<DomainException>();
        emptyProductId.Should().Throw<DomainException>();
        invalidPrice.Should().Throw<DomainException>();
        invalidQuantity.Should().Throw<DomainException>();
    }
}
