namespace Challange.Api.Middlewares;

public class CorrellationMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";
    private const string CorrelationIdLogProperty = "CorrelationId";
    private readonly ILogger<CorrellationMiddleware> _logger;
    private readonly RequestDelegate _next;

    public CorrellationMiddleware(RequestDelegate next, ILogger<CorrellationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        using var _ = _logger.BeginScope(new Dictionary<string, string>
               {
                   { CorrelationIdLogProperty, correlationId }
               });

        await _next(context);
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }
        context.Response.Headers.TryAdd(CorrelationIdHeaderName, correlationId);
        return correlationId.ToString();
    }
}