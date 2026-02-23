using Challenge.Application.Commons;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Challenge.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results.SelectMany(r => r.Errors).Where(e => e is not null).ToList();

        if (failures.Count == 0)
            return await next();

        var details = failures
            .Select(f => new KeyValuePair<string, string>(f.PropertyName, f.ErrorMessage))
            .ToList();

        _logger.LogWarning("Validation failed for {RequestType}: {@Details}", typeof(TRequest).Name, details);

        return CreateValidationResponse(details);
    }

    private static TResponse CreateValidationResponse(List<KeyValuePair<string, string>> details)
    {
        var responseType = typeof(TResponse);

        var valueType = responseType.GetGenericArguments()[0];

        var method = typeof(Result<>)
            .MakeGenericType(valueType)
            .GetMethod(nameof(Result<object>.Validation),
                new[] { typeof(string), typeof(List<KeyValuePair<string, string>>) });

        return (TResponse)method?.Invoke(null, new object[] { "Validation failed", details })!;
    }
}