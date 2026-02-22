using Challenge.Domain.Constants;
using Challenge.Domain.Exceptions;

namespace Challenge.Domain.Entities;

public class User : BaseEntity
{
    public string Login { get; private set; }
    public string NormalizedLogin { get; private set; }
    public string PasswordHash { get; private set; }

    public User(string login, string passwordHash) : base()
    {
        DomainException.ThrowIf(string.IsNullOrWhiteSpace(login), "Login cannot be empty.");
        DomainException.ThrowIf(login.Length < UserConstants.MinLoginLength || login.Length > UserConstants.MaxLoginLength, $"Login must be between {UserConstants.MinLoginLength} and {UserConstants.MaxLoginLength} characters.");
        DomainException.ThrowIf(string.IsNullOrWhiteSpace(passwordHash), "Password hash cannot be empty.");
        DomainException.ThrowIf(passwordHash.Length > UserConstants.MaxPasswordHashLength, $"Password hash must not exceed {UserConstants.MaxPasswordHashLength} characters.");

        Login = login;
        NormalizedLogin = login.ToUpperInvariant();
        PasswordHash = passwordHash;
    }

    public void SetPasswordHash(string passwordHash)
    {
        DomainException.ThrowIf(string.IsNullOrWhiteSpace(passwordHash), "Password hash cannot be empty.");

        PasswordHash = passwordHash;
        UpdateTimestamps();
    }

    private User()
    {
    }
}