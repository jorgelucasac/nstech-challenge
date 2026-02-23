using Challenge.Application.Features.Commands.Auth.RegisterUser;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Challenge.Application.Contracts.Repositories;
using Challenge.Application.Contracts.Services;

namespace Challenge.UnitTests.Application.Handlers;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasherService> _passwordHasher = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _unitOfWork.Object,
            NullLogger<RegisterUserCommandHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidation_WhenUserAlreadyExists()
    {
        _userRepository
            .Setup(repo => repo.ExistsByLoginAsync("login", CancellationToken.None))
            .ReturnsAsync(true);
        var result = await _handler.HandleAsync(new RegisterUserCommand("login", "password"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(400);
    }

    [Fact]
    public async Task HandleAsync_ShouldRegisterUser_WhenLoginAvailable()
    {
        _userRepository
            .Setup(repo => repo.ExistsByLoginAsync("login", CancellationToken.None))
            .ReturnsAsync(false);
        _userRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Challenge.Domain.Entities.User>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        _unitOfWork.Setup(work => work.CommitAsync(CancellationToken.None)).ReturnsAsync(1);

        _passwordHasher.Setup(service => service.HashPassword("password")).Returns("hashed");

        var result = await _handler.HandleAsync(new RegisterUserCommand("login", "password"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Login.Should().Be("login");
        _userRepository.Verify(repo => repo.AddAsync(It.Is<Challenge.Domain.Entities.User>(u => u.PasswordHash == "hashed"), CancellationToken.None), Times.Once);
        _unitOfWork.Verify(work => work.CommitAsync(CancellationToken.None), Times.Once);
    }
}
