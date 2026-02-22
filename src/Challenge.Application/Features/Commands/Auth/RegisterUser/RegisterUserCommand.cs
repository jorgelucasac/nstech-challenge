using Challenge.Application.Commons;
using MediatR;

namespace Challenge.Application.Features.Commands.Auth.RegisterUser;

public sealed record RegisterUserCommand(string Login, string Password) : IRequest<Result<RegisterUserResponse>>;