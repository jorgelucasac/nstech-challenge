using Challenge.Domain.Constants;
using Challenge.Domain.Entities;
using Challenge.Domain.Exceptions;
using FluentAssertions;
using System.Threading;

namespace Challenge.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializeUser()
    {
        var user = new User("login", "hash");

        user.Login.Should().Be("login");
        user.NormalizedLogin.Should().Be("LOGIN");
        user.PasswordHash.Should().Be("hash");
    }

    [Fact]
    public void Constructor_ShouldThrowWhenLoginIsInvalid()
    {
        Action emptyLogin = () => new User("", "hash");
        Action tooShort = () => new User(new string('a', UserConstants.MinLoginLength - 1), "hash");
        Action tooLong = () => new User(new string('a', UserConstants.MaxLoginLength + 1), "hash");

        emptyLogin.Should().Throw<DomainException>();
        tooShort.Should().Throw<DomainException>();
        tooLong.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_ShouldThrowWhenPasswordHashIsInvalid()
    {
        Action emptyHash = () => new User("login", "");
        Action tooLong = () => new User("login", new string('a', UserConstants.MaxPasswordHashLength + 1));

        emptyHash.Should().Throw<DomainException>();
        tooLong.Should().Throw<DomainException>();
    }

    [Fact]
    public void SetPasswordHash_ShouldUpdateHashAndTimestamp()
    {
        var user = new User("login", "hash");
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(5);
        user.SetPasswordHash("new-hash");

        user.PasswordHash.Should().Be("new-hash");
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void SetPasswordHash_ShouldThrowWhenInvalid()
    {
        var user = new User("login", "hash");

        Action act = () => user.SetPasswordHash(" ");

        act.Should().Throw<DomainException>();
    }
}
