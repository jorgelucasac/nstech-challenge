using Challenge.Application.Features.Commands.Auth.GenerateToken;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Contracts.Services;
using Challenge.Application.Dtos;
using Challenge.Domain.Entities;

namespace Challenge.UnitTests.Application.Handlers;

public class GenerateTokenCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasherService> _passwordHasher = new();
    private readonly Mock<ITokenService> _tokenService = new();

    private readonly GenerateTokenCommandHandler _handler;

    public GenerateTokenCommandHandlerTests()
    {
        _handler = new GenerateTokenCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _tokenService.Object,
            NullLogger<GenerateTokenCommandHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        _userRepository
            .Setup(repo => repo.GetByLoginAsync("login", CancellationToken.None))
            .ReturnsAsync((User?)null);
        var result = await _handler.HandleAsync(new GenerateTokenCommand("login", "password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(401);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUnauthorized_WhenPasswordInvalid()
    {
        var user = new User("login", "hash");
        _userRepository
            .Setup(repo => repo.GetByLoginAsync("login", CancellationToken.None))
            .ReturnsAsync(user);

        _passwordHasher.Setup(service => service.VerifyPassword("password", user.PasswordHash)).Returns(false);

        var result = await _handler.HandleAsync(new GenerateTokenCommand("login", "password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(401);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnToken_WhenCredentialsValid()
    {
        var user = new User("login", "hash");
        _userRepository
            .Setup(repo => repo.GetByLoginAsync(user.Login, CancellationToken.None))
            .ReturnsAsync(user);

        _passwordHasher.Setup(service => service.VerifyPassword("password", user.PasswordHash)).Returns(true);

        var tokenResponse = new TokenResponse("access", DateTime.UtcNow.AddMinutes(30));
        _tokenService.Setup(service => service.GenerateToken(user.Id, user.Login)).Returns(tokenResponse);

        var result = await _handler.HandleAsync(new GenerateTokenCommand(user.Login, "password"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("access");
        result.Value.ExpiresAtUtc.Should().Be(tokenResponse.ExpiresAtUtc);
    }
}