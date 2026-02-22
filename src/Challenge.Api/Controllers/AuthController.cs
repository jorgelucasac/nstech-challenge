using Challenge.Application.Dtos;
using Challenge.Application.Features.Commands.Auth.GenerateToken;
using Challenge.Application.Features.Commands.Auth.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers;

[Route("api/v{version:apiVersion}/auth")]
public class AuthController(IMediator mediator) : ControllerMain
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RegisterUserCommand(request.Login, request.Password), cancellationToken);

        if (result.IsSuccess)
        {
            return Created(string.Empty, result.Value);
        }

        return ErrorResponse(result.Error);
    }

    [HttpPost("token")]
    [ProducesResponseType(typeof(GenerateTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GenerateTokenAsync([FromBody] TokenRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GenerateTokenCommand(request.Login, request.Password), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return ErrorResponse(result.Error);
    }
}