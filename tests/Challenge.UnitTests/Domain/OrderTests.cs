using Challenge.Domain.Constants;
using Challenge.Domain.Entities;
using Challenge.Domain.Enums;
using Challenge.Domain.Exceptions;
using FluentAssertions;
using System.Reflection;
using System.Threading;

namespace Challenge.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldInitializeOrder()
    {
        var userId = Guid.NewGuid();

        var order = new Order(userId, "usd");

        order.UserId.Should().Be(userId);
        order.Currency.Should().Be("USD");
        order.Status.Should().Be(OrderStatus.Placed);
        order.Items.Should().BeEmpty();
        order.Total.Should().Be(0m);
    }

    [Fact]
    public void Constructor_ShouldThrowWhenInvalid()
    {
        Action emptyUser = () => new Order(Guid.Empty, "usd");
        Action emptyCurrency = () => new Order(Guid.NewGuid(), " ");
        Action invalidCurrency = () => new Order(Guid.NewGuid(), new string('a', OrderConstants.CurrencyLength + 1));

        emptyUser.Should().Throw<DomainException>();
        emptyCurrency.Should().Throw<DomainException>();
        invalidCurrency.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddItems_ShouldAddItemsAndRecalculateTotal()
    {
        var order = new Order(Guid.NewGuid(), "usd");
        var items = new List<OrderItem>
        {
            new OrderItem(order.Id, Guid.NewGuid(), 10m, 2),
            new OrderItem(order.Id, Guid.NewGuid(), 5m, 1)
        };
        var originalUpdatedAt = order.UpdatedAt;

        Thread.Sleep(5);
        order.AddItems(items);

        order.Items.Should().HaveCount(2);
        order.Total.Should().Be(25m);
        order.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void AddItems_ShouldThrowWhenNullOrEmpty()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        Action nullItems = () => order.AddItems(null!);
        Action emptyItems = () => order.AddItems([]);

        nullItems.Should().Throw<DomainException>();
        emptyItems.Should().Throw<DomainException>();
    }

    [Fact]
    public void CanConfirm_ShouldReturnTrueWhenPlacedOrConfirmed()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        order.CanConfirm().Should().BeTrue();

        order.TryConfirm();
        order.CanConfirm().Should().BeTrue();
    }

    [Fact]
    public void CanConfirm_ShouldReturnFalseWhenCanceled()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        order.TryCancel();

        order.CanConfirm().Should().BeFalse();
    }

    [Fact]
    public void TryConfirm_ShouldConfirmWhenPlaced()
    {
        var order = new Order(Guid.NewGuid(), "usd");
        var originalUpdatedAt = order.UpdatedAt;

        Thread.Sleep(5);
        var result = order.TryConfirm();

        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void TryConfirm_ShouldReturnTrueWhenAlreadyConfirmed()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        order.TryConfirm();
        var result = order.TryConfirm();

        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void TryConfirm_ShouldReturnFalseWhenNotPlaced()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        order.TryCancel();
        var result = order.TryConfirm();

        result.Should().BeFalse();
        order.Status.Should().Be(OrderStatus.Canceled);
    }

    [Fact]
    public void CanCancel_ShouldReturnTrueWhenPlacedConfirmedOrCanceled()
    {
        var placed = new Order(Guid.NewGuid(), "usd");
        var confirmed = new Order(Guid.NewGuid(), "usd");
        confirmed.TryConfirm();
        var canceled = new Order(Guid.NewGuid(), "usd");
        canceled.TryCancel();

        placed.CanCancel().Should().BeTrue();
        confirmed.CanCancel().Should().BeTrue();
        canceled.CanCancel().Should().BeTrue();
    }

    [Fact]
    public void CanCancel_ShouldReturnFalseWhenDraft()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        SetStatus(order, OrderStatus.Draft);

        order.CanCancel().Should().BeFalse();
    }

    [Fact]
    public void TryCancel_ShouldCancelWhenPlaced()
    {
        var order = new Order(Guid.NewGuid(), "usd");
        var originalUpdatedAt = order.UpdatedAt;

        Thread.Sleep(5);
        var result = order.TryCancel();

        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Canceled);
        order.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void TryCancel_ShouldReturnTrueWhenAlreadyCanceled()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        order.TryCancel();
        var result = order.TryCancel();

        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Canceled);
    }

    [Fact]
    public void TryCancel_ShouldReturnFalseWhenNotPlacedOrConfirmed()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        SetStatus(order, OrderStatus.Draft);
        var result = order.TryCancel();

        result.Should().BeFalse();
        order.Status.Should().Be(OrderStatus.Draft);
    }

    [Fact]
    public void RecalculateTotal_ShouldSumItems()
    {
        var order = new Order(Guid.NewGuid(), "usd");

        order.Items.Add(new OrderItem(order.Id, Guid.NewGuid(), 10m, 1));
        order.Items.Add(new OrderItem(order.Id, Guid.NewGuid(), 2.5m, 4));

        order.RecalculateTotal();

        order.Total.Should().Be(20m);
    }

    private static void SetStatus(Order order, OrderStatus status)
    {
        var property = typeof(Order).GetProperty("Status", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        property!.SetValue(order, status);
    }
}
