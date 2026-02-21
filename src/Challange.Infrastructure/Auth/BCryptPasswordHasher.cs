using Challange.Application.Contracts.Services;

namespace Challange.Infrastructure.Auth;

public class BCryptPasswordHasher : IPasswordHasherService
{
    private const int WorkFactor = 12;

    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool VerifyPassword(string password, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}