using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Features.Queries.Products.ListProducts;
using Challenge.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Challenge.UnitTests.Application.Handlers;

public class ListProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();

    private readonly ListProductsQueryHandler _handler;

    public ListProductsQueryHandlerTests()
    {
        _handler = new ListProductsQueryHandler(
            _productRepository.Object,
            NullLogger<ListProductsQueryHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedProducts()
    {
        var product = new Product("Rice", 10m, 5);
        _productRepository
            .Setup(repo => repo.ListAsync("Rice", 1, 10, CancellationToken.None))
            .ReturnsAsync((Products: (IReadOnlyList<Product>)new List<Product> { product }, TotalCount: 1));

        var result = await _handler.HandleAsync(new ListProductsQuery("Rice", 1, 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
        result.Value.Products.Should().HaveCount(1);
        result.Value.Products[0].Id.Should().Be(product.Id);
        result.Value.Products[0].Description.Should().Be("Rice");
    }
}
