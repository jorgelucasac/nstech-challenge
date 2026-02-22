using Challange.Application.Commons;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Challange.Application.Features;

public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
     where TRequest : IRequest<Result<TResponse>> where TResponse : class
{
    protected readonly ILogger<BaseHandler<TRequest, TResponse>> Logger;

    protected BaseHandler(ILogger<BaseHandler<TRequest, TResponse>> logger)
    {
        Logger = logger;
    }

    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        Logger.LogInformation("Handling {RequestType}", typeof(TRequest).Name);
        var result = await HandleAsync(request, cancellationToken);
        var endTime = DateTime.UtcNow;
        Logger.LogInformation("Handled {RequestType} in {ElapsedMilliseconds}ms", typeof(TRequest).Name, (endTime - startTime).TotalMilliseconds);
        return result;
    }

    public abstract Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}