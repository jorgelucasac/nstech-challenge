using Challange.Domain.Entities;

namespace Challange.Application.Contracts.Repositories;

public interface IProductRepository
{
    Task<Dictionary<Guid, Product>> GetByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
}