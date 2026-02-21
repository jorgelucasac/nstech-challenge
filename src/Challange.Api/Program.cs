using Challange.Api.Extensions.Swagger;
using Challange.Infrastructure;
using Challange.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerCustom();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await DbInitializer.ApplyMigrationsAsync(app.Services, app.Lifetime.ApplicationStopping);
app.UseSwaggerCustom();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync(app.Lifetime.ApplicationStopping);