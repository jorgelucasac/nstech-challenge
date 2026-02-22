using Asp.Versioning;
using Challenge.Api.Transport;
using Challenge.Application.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
[ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
public abstract class ControllerMain : ControllerBase
{
    protected IActionResult ErrorResponse(Error error)
    {
        var statusCode = error.Code;
        var response = ApiErrorResponse.FromError(error);

        return statusCode switch
        {
            StatusCodes.Status404NotFound => NotFound(response),
            StatusCodes.Status422UnprocessableEntity => UnprocessableEntity(response),
            StatusCodes.Status401Unauthorized => Unauthorized(response),
            StatusCodes.Status400BadRequest => BadRequest(response),
            _ => BadRequest(response)
        };
    }
}