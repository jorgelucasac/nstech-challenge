using Challenge.Api.Middlewares;

namespace Challenge.Api.Extensions;

public static class CustomMiddlewareExtensions
{
    public static void UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrellationMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}