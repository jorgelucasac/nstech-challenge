using Challenge.Domain.Entities;
using Challenge.Domain.Enums;

namespace Challenge.Application.Contracts.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task<Order?> GetByIdWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task<Order?> GetByIdReadOnlyWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Order> Orders, int TotalCount)> ListAsync(Guid? userId, OrderStatus? status, DateTime? from, DateTime? to, int page, int pageSize, CancellationToken cancellationToken = default);
}