using Challenge.Domain.Constants;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using FluentAssertions;
using System.Threading;

namespace Challenge.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldInitializeProduct()
    {
        var product = new Product("Item", 10m, 5);

        product.Description.Should().Be("Item");
        product.UnitPrice.Should().Be(10m);
        product.AvailableQuantity.Should().Be(5);
    }

    [Fact]
    public void Constructor_ShouldThrowWhenValuesAreInvalid()
    {
        Action emptyDescription = () => new Product(" ", 10m, 5);
        Action longDescription = () => new Product(new string('a', ProductConstants.MaxDescriptionLength + 1), 10m, 5);
        Action invalidPrice = () => new Product("Item", ProductConstants.MinUnitPrice - 0.01m, 5);
        Action invalidQuantity = () => new Product("Item", 10m, ProductConstants.MinAvailableQuantity - 1);

        emptyDescription.Should().Throw<DomainException>();
        longDescription.Should().Throw<DomainException>();
        invalidPrice.Should().Throw<DomainException>();
        invalidQuantity.Should().Throw<DomainException>();
    }

    [Fact]
    public void DecreaseStock_ShouldReduceQuantityAndUpdateTimestamp()
    {
        var product = new Product("Item", 10m, 5);
        var originalUpdatedAt = product.UpdatedAt;

        Thread.Sleep(5);
        product.DecreaseStock(2);

        product.AvailableQuantity.Should().Be(3);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void DecreaseStock_ShouldThrowWhenInvalid()
    {
        var product = new Product("Item", 10m, 5);

        Action nonPositive = () => product.DecreaseStock(0);
        Action tooLarge = () => product.DecreaseStock(10);

        nonPositive.Should().Throw<DomainException>();
        tooLarge.Should().Throw<DomainException>();
    }

    [Fact]
    public void IncreaseStock_ShouldIncreaseQuantityAndUpdateTimestamp()
    {
        var product = new Product("Item", 10m, 5);
        var originalUpdatedAt = product.UpdatedAt;

        Thread.Sleep(5);
        product.IncreaseStock(2);

        product.AvailableQuantity.Should().Be(7);
        product.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void IncreaseStock_ShouldThrowWhenInvalid()
    {
        var product = new Product("Item", 10m, 5);

        Action act = () => product.IncreaseStock(0);

        act.Should().Throw<DomainException>();
    }
}
