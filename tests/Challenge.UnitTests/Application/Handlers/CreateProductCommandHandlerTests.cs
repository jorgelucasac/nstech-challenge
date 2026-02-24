using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Features.Commands.Products.CreateProduct;
using Challenge.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Challenge.UnitTests.Application.Handlers;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _handler = new CreateProductCommandHandler(
            _productRepository.Object,
            _unitOfWork.Object,
            NullLogger<CreateProductCommandHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateProduct_WhenValidRequest()
    {
        _productRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Product>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(work => work.CommitAsync(CancellationToken.None)).ReturnsAsync(1);

        var request = new CreateProductCommand("Rice", 10m, 5);
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be("Rice");
        result.Value.UnitPrice.Should().Be(10m);
        result.Value.AvailableQuantity.Should().Be(5);
        _productRepository.Verify(repo => repo.AddAsync(
            It.Is<Product>(p => p.Description == "Rice" && p.UnitPrice == 10m && p.AvailableQuantity == 5),
            CancellationToken.None), Times.Once);
        _unitOfWork.Verify(work => work.CommitAsync(CancellationToken.None), Times.Once);
    }
}
