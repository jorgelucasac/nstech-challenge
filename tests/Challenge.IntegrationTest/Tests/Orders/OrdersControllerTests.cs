using Challenge.Api.Transport.Orders;
using Challenge.Api.Transport.Products;
using Challenge.Application.Features.Commands.Orders;
using Challenge.Application.Features.Queries.Orders.ListOrders;
using Challenge.Application.Features.Shared.Products;
using Challenge.Domain.Enums;
using Challenge.IntegrationTest.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Challenge.IntegrationTest.Tests.Orders;

public class OrdersControllerTests(PostgressContainerFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedOrder()
    {
        var customerId = await AuthorizeAndGetUserIdAsync();
        var product = await CreateProductAsync();

        var request = new CreateOrderRequest(customerId, "USD", [new CreateOrderItemRequest(product.Id, 2)]);
        var response = await Client!.PostAsJsonAsync("api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<OrderResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.CustomerId.Should().Be(customerId);
        body.Status.Should().Be(OrderStatus.Placed);
        body.Currency.Should().Be("USD");
        body.Items.Should().ContainSingle(i => i.ProductId == product.Id && i.Quantity == 2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder()
    {
        var customerId = await AuthorizeAndGetUserIdAsync();
        var order = await CreateOrderAsync(customerId);

        var response = await Client!.GetAsync($"api/v1/orders/{order.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<OrderResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().Be(order.Id);
        body.CustomerId.Should().Be(order.CustomerId);
        body.Status.Should().Be(order.Status);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnOrders()
    {
        var customerId = await AuthorizeAndGetUserIdAsync();
        var order = await CreateOrderAsync(customerId);

        var response = await Client!.GetAsync($"api/v1/orders?customerId={order.CustomerId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<OrderListResponse>();
        body.Should().NotBeNull();
        body!.Orders.Should().ContainSingle(o => o.Id == order.Id);
        body.Page.Should().Be(1);
        body.PageSize.Should().Be(10);
        body.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ConfirmAsync_ShouldReturnConfirmedOrder()
    {
        var customerId = await AuthorizeAndGetUserIdAsync();
        var order = await CreateOrderAsync(customerId);

        var response = await Client!.PostAsync($"api/v1/orders/{order.Id}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<OrderResponse>();
        body.Should().NotBeNull();
        body!.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task CancelAsync_ShouldReturnCanceledOrder()
    {
        var customerId = await AuthorizeAndGetUserIdAsync();
        var order = await CreateOrderAsync(customerId);

        var response = await Client!.PostAsync($"api/v1/orders/{order.Id}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<OrderResponse>();
        body.Should().NotBeNull();
        body!.Status.Should().Be(OrderStatus.Canceled);
    }

    private async Task<Guid> AuthorizeAndGetUserIdAsync()
    {
        Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", IntegrationTestBaseHelpers.Jwt);
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(IntegrationTestBaseHelpers.Jwt);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        userIdClaim.Should().NotBeNull("Token should contain a 'sub' claim with the user ID");
        return Guid.Parse(userIdClaim!.Value);
    }

    private async Task<ProductResponse> CreateProductAsync()
    {
        var request = new CreateProductRequest($"Product-{Guid.NewGuid():N}", 15.75m, 20);
        var response = await Client!.PostAsJsonAsync("api/v1/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();
        body.Should().NotBeNull();
        return body!;
    }

    private async Task<OrderResponse> CreateOrderAsync(Guid customerId)
    {
        var product = await CreateProductAsync();
        var request = new CreateOrderRequest(customerId, "USD", [new CreateOrderItemRequest(product.Id, 1)]);
        var response = await Client!.PostAsJsonAsync("api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<OrderResponse>();
        body.Should().NotBeNull();
        return body!;
    }
}