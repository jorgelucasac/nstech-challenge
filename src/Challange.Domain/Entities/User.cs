using Challange.Domain.Exceptions;

namespace Challange.Domain.Entities;

public class User : BaseEntity
{
    public string Login { get; private set; }
    public string NormalizedLogin { get; private set; }
    public string PasswordHash { get; private set; }

    public User(string login, string passwordHash) : base()
    {
        DomainException.ThrowIf(string.IsNullOrWhiteSpace(login), "Login cannot be empty.");
        DomainException.ThrowIf(string.IsNullOrWhiteSpace(passwordHash), "Password hash cannot be empty.");

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
    { }
}