using Challenge.Application.Commons;
using Challenge.Application.Features.Commands.Auth.GenerateToken;
using Challenge.IntegrationTest.Fixtures;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.IntegrationTest.Tests.DependencyInjection;

public class DependencyInjectionTests(PostgressContainerFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public void ApplicationServices_PipelineBehaviorRegistered_ShouldWork()
    {
        var behavior = Factory?.Services.CreateScope()
            .ServiceProvider
            .GetService<IPipelineBehavior<GenerateTokenCommand, Result<GenerateTokenResponse>>>();

        behavior?.Should().NotBeNull();
    }
}