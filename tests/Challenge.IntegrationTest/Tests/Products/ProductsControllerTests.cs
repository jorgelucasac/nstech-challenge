using Challenge.Api.Transport.Products;
using Challenge.Application.Features.Queries.Products.ListProducts;
using Challenge.Application.Features.Shared.Products;
using Challenge.IntegrationTest.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Challenge.IntegrationTest.Tests.Products;

public class ProductsControllerTests(PostgressContainerFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedProduct()
    {
        Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", IntegrationTestBaseHelpers.Jwt);

        var request = new CreateProductRequest($"Product-{Guid.NewGuid():N}", 19.99m, 10);
        var response = await Client.PostAsJsonAsync("api/v1/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.Description.Should().Be(request.Description);
        body.UnitPrice.Should().Be(request.UnitPrice);
        body.AvailableQuantity.Should().Be(request.AvailableQuantity);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnProducts()
    {
        Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", IntegrationTestBaseHelpers.Jwt);

        var description = $"Product-{Guid.NewGuid():N}";
        var request = new CreateProductRequest(description, 10.50m, 5);
        var createResponse = await Client.PostAsJsonAsync("api/v1/products", request);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var listResponse = await Client.GetAsync($"api/v1/products?name={description}");

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await listResponse.Content.ReadFromJsonAsync<ProductListResponse>();
        body.Should().NotBeNull();
        body!.Products.Should().ContainSingle(p => p.Description == description);
        body.Page.Should().Be(1);
        body.PageSize.Should().Be(10);
        body.TotalCount.Should().BeGreaterThan(0);
    }
}