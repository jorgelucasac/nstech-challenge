using Challenge.Api.Transport.Auth;
using Challenge.Application.Features.Commands.Auth.GenerateToken;
using Challenge.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Challenge.IntegrationTest.Fixtures;

[Collection(nameof(DatabaseCollection))]
public abstract class IntegrationTestBase(PostgressContainerFixture db) : IAsyncLifetime
{
    public HttpClient? Client;
    private readonly PostgressContainerFixture _db = db;
    protected CustomWebApplicationFactory? Factory;
    protected AppDbContext? TestDbContext;

    public async Task InitializeAsync()
    {
        Factory = new CustomWebApplicationFactory(_db.ConnectionString);
        TestDbContext = Factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        Client = Factory.CreateClient();
        if (string.IsNullOrWhiteSpace(IntegrationTestBaseHelpers.Jwt))
        {
            IntegrationTestBaseHelpers.Jwt = await AuthenticateAndGetTokenAsync();
        }
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

    private async Task<string> AuthenticateAndGetTokenAsync()
    {
        //foreach until status code is 200, because the application may not be ready yet

        var tokenResponse = await Client!.PostAsJsonAsync("api/v1/auth/token", new TokenRequest("admin", "admin01"));

        var body = await tokenResponse.Content.ReadFromJsonAsync<GenerateTokenResponse>();

        return body?.Token ?? string.Empty;
    }
}