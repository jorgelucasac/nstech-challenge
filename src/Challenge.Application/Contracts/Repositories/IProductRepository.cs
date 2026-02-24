using Challenge.Domain.Entities;

namespace Challenge.Application.Contracts.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, Product>> GetByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Product> Products, int TotalCount)> ListAsync(string? name, int page, int pageSize, CancellationToken cancellationToken = default);
}