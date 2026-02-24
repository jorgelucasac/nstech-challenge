using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace Challenge.IntegrationTest.Fixtures;

public sealed class PostgressContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    public string ConnectionString => _container.GetConnectionString();

    public PostgressContainerFixture()
    {
        _container = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("challenge_db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5432, true)
            .WithCleanUp(true)
             .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilExternalTcpPortIsAvailable(5432)
            )
            .Build();
    }

    public async Task InitializeAsync()
    {
        if (_container is null)
        {
            return;
        }
        try
        {
            await _container.StartAsync();
        }
        catch
        {
            var logs = await _container.GetLogsAsync();
            throw new Exception($"Postgress container failed to start.\n\nLOGS:\n{logs}");
        }
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}