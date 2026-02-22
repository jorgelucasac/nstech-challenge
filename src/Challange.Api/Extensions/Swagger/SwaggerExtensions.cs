using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace Challange.Api.Extensions.Swagger;

public static class SwaggerExtensions
{
    public static void AddSwaggerCustom(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Challange API",
                Version = "v1"
            });

            options.OperationFilter<AddCorrelationHeaderOperationFilter>();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token without the 'Bearer ' prefix </br> Ex: your_jwt_token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void UseSwaggerCustom(this WebApplication app)
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });
    }
}