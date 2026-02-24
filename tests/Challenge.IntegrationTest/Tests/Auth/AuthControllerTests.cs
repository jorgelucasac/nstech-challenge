using Challenge.Api.Transport.Auth;
using Challenge.Application.Features.Commands.Auth.GenerateToken;
using Challenge.Application.Features.Commands.Auth.RegisterUser;
using Challenge.IntegrationTest.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Challenge.IntegrationTest.Tests.Auth;

public class AuthControllerTests(PostgressContainerFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task RegisterAsync_ShouldReturnCreatedWithUser()
    {
        var request = new RegisterUserRequest($"user-{Guid.NewGuid():N}", "Pass@12345");

        var response = await Client!.PostAsJsonAsync("api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.Login.Should().Be(request.Login);
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturnTokenForValidCredentials()
    {
        var request = new RegisterUserRequest($"user-{Guid.NewGuid():N}", "Pass@12345");
        var registerResponse = await Client!.PostAsJsonAsync("api/v1/auth/register", request);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var tokenResponse = await Client!.PostAsJsonAsync("api/v1/auth/token", new TokenRequest(request.Login, request.Password));

        tokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await tokenResponse.Content.ReadFromJsonAsync<GenerateTokenResponse>();
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturnUnauthorizedForInvalidCredentials()
    {
        var request = new RegisterUserRequest($"user-{Guid.NewGuid():N}", "Pass@12345");
        var registerResponse = await Client!.PostAsJsonAsync("api/v1/auth/register", request);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var tokenResponse = await Client!.PostAsJsonAsync("api/v1/auth/token", new TokenRequest(request.Login, "Wrong@123"));

        tokenResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}