using Challenge.Api.Transport.Auth;
using Challenge.Application.Features.Commands.Auth.GenerateToken;
using Challenge.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Challenge.IntegrationTest.Fixtures;

[Collection(nameof(DatabaseCollection))]
public abstract class IntegrationTestBase(PostgressContainerFixture db) : IAsyncLifetime
{
    public HttpClient? Client;
    private readonly PostgressContainerFixture _db = db;
    protected CustomWebApplicationFactory? Factory;
    protected AppDbContext? TestDbContext;

    public Task InitializeAsync()
    {
        Factory = new CustomWebApplicationFactory(_db.ConnectionString);
        TestDbContext = Factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        Client = Factory.CreateClient();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (TestDbContext != null)
        {
            await TestDbContext.DisposeAsync();
        }
        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }
    }

    protected async Task<string> RegisterAndGetTokenAsync()
    {
        var login = $"user-{Guid.NewGuid():N}";
        const string password = "Pass@12345";
        var registerResponse = await Client!.PostAsJsonAsync("api/v1/auth/register", new RegisterUserRequest(login, password));

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var tokenResponse = await Client!.PostAsJsonAsync("api/v1/auth/token", new TokenRequest(login, password));

        tokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await tokenResponse.Content.ReadFromJsonAsync<GenerateTokenResponse>();
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();

        return body.Token;
    }
}

[CollectionDefinition(nameof(DatabaseCollection))]
public class DatabaseCollection : ICollectionFixture<PostgressContainerFixture>
{
}