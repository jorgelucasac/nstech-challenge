using Challenge.Domain.Entities;

namespace Challenge.Application.Contracts.Repositories;

public interface IProductRepository
{
    Task<Dictionary<Guid, Product>> GetByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
}