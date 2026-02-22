using Challange.Api.Transport;
using System.Net.Mime;

namespace Challange.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private const string ErorMessage = "An unexpected error occurred. Please try again later.";
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Error on HttpRequest ErrorMessage: {Message}", exception.Message);

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var response = new ApiErrorResponse
        {
            Message = ErorMessage
        };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}