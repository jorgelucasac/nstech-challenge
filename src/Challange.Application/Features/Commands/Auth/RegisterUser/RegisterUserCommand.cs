using Challange.Application.Commons;
using MediatR;

namespace Challange.Application.Features.Commands.Auth.RegisterUser;

public sealed record RegisterUserCommand(string Login, string Password) : IRequest<Result<RegisterUserResponse>>;