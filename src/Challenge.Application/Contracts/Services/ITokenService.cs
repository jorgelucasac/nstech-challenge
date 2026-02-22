using Challenge.Application.Dtos;

namespace Challenge.Application.Contracts.Services;

public interface ITokenService
{
    TokenResponse GenerateToken(Guid userId, string username);
}