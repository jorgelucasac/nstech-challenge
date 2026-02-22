using Challange.Application.Commands.Auth.GenerateToken;
using Challange.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Challange.Api.Controllers;

[Route("api/v{version:apiVersion}/auth")]
public class AuthController(IMediator mediator) : ControllerMain
{
    [HttpPost("token")]
    [ProducesResponseType(typeof(GenerateTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GenerateTokenAsync([FromBody] TokenRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GenerateTokenCommand(request.Login, request.Password), cancellationToken);
        return ErrorResponse(result.Error);
    }
}