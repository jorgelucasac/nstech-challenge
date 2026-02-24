using Challenge.Application.Contracts.Repositories;
using Challenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Persistence.Repositories;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }

    public async Task<Dictionary<Guid, Product>> GetByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken)
    {
        var ids = productIds.Distinct().ToList();
        return await dbContext.Products
            .Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Product> Products, int TotalCount)> ListAsync(string? name, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Description.Contains(name));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var products = await query.OrderBy(p => p.Description)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }
}