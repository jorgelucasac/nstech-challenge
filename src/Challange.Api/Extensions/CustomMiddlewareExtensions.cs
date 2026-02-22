using Challange.Api.Middlewares;

namespace Challange.Api.Extensions;

public static class CustomMiddlewareExtensions
{
    public static void UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrellationMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}