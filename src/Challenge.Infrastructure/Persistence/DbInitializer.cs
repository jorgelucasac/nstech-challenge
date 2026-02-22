using Challenge.Domain.Entities;
using Challenge.Infrastructure.Auth;
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
            //create the database if it doesn't exist and apply any pending migrations
            if (!await dbContext.Database.CanConnectAsync(cancellationToken))
            {
                await dbContext.Database.EnsureCreatedAsync(cancellationToken);
                await SeedDataAsync(dbContext, cancellationToken);
            }
            else
            {
                Console.WriteLine("Applying pending migrations...");
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while creating the database: {e.Message}");
        }
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken))
            return;

        dbContext.Products.Add(new Product("Rice", 30m, 100));
        dbContext.Products.Add(new Product("Beans", 5.90m, 200));
        dbContext.Products.Add(new Product("Sugar", 10.50m, 500));

        dbContext.Users.Add(new User("admin", new BCryptPasswordHasher().HashPassword("admin01")));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}