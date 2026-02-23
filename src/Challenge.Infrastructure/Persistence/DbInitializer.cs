using Challenge.Application.Contracts.Services;
using Challenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            Console.WriteLine("Applying pending migrations...");
            await dbContext.Database.MigrateAsync(cancellationToken);

            await SeedDataAsync(dbContext, scope, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while creating the database: {e.Message}");
        }
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, IServiceScope scope, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken))
            return;

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

        dbContext.Products.Add(new Product("Rice", 30m, 100));
        dbContext.Products.Add(new Product("Beans", 5.90m, 200));
        dbContext.Products.Add(new Product("Sugar", 10.50m, 500));

        dbContext.Users.Add(new User("admin", passwordHasher.HashPassword("admin01")));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}