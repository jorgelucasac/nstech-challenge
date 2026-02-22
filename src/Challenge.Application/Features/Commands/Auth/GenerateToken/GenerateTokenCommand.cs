using Challenge.Application.Commons;
using MediatR;

namespace Challenge.Application.Features.Commands.Auth.GenerateToken;

public sealed record GenerateTokenCommand(string Login, string Password) : IRequest<Result<GenerateTokenResponse>>;