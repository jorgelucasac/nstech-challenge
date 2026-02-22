using Challange.Application.Commons;
using MediatR;

namespace Challange.Application.Commands.Auth.GenerateToken;

public sealed record GenerateTokenCommand(string Login, string Password) : IRequest<Result<GenerateTokenResponse>>;