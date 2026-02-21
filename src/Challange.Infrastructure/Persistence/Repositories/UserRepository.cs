using Challange.Application.Contracts.Repositories;
using Challange.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Challange.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var normalized = login.ToUpperInvariant();
        return await dbContext
            .Users
            .FirstOrDefaultAsync(x => x.NormalizedLogin == normalized, cancellationToken);
    }

    public async Task<bool> ExistsByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var normalized = login.ToUpperInvariant();
        return await dbContext.Users
            .AnyAsync(x => x.NormalizedLogin == normalized, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }
}