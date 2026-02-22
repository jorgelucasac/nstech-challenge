using Challange.Api.Extensions;
using Challange.Api.Extensions.Swagger;
using Challange.Application;
using Challange.Infrastructure;
using Challange.Infrastructure.Persistence;

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync(app.Lifetime.ApplicationStopping);