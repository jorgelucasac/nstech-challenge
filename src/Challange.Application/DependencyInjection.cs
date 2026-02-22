using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Challange.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddMediatR(cfg => cfg.AsScoped(), assembly);
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}