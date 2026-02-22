using Challenge.Api.Extensions;
using Challenge.Api.Extensions.Swagger;
using Challenge.Application;
using Challenge.Infrastructure;
using Challenge.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddVersioning();
builder.Services.AddSwaggerCustom();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Host.AddSerilog();

var app = builder.Build();

await DbInitializer.ApplyMigrationsAsync(app.Services, app.Lifetime.ApplicationStopping);
app.UseSwaggerCustom();
app.UseCustomMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();