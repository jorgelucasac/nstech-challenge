using Challenge.Application.Contracts.Repositories;
using Challenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var normalized = login.ToUpperInvariant();
        return await dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NormalizedLogin == normalized, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AnyAsync(x => x.Id == id, cancellationToken);
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