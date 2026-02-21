using Challange.Application.Dtos;

namespace Challange.Application.Contracts.Services;

public interface ITokenService
{
    TokenResponse GenerateToken(Guid userId, string username);
}