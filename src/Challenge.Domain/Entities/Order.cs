using Challenge.Domain.Constants;
using Challenge.Domain.Enums;
using Challenge.Domain.Exceptions;

namespace Challenge.Domain.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string Currency { get; private set; } = null!;
    public decimal Total { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];
    public User? User { get; private set; }

    public Order(Guid userId, string currency) : base()
    {
        DomainException.ThrowIf(userId == Guid.Empty, "User ID cannot be empty.");
        DomainException.ThrowIf(string.IsNullOrWhiteSpace(currency), "Currency cannot be empty.");
        DomainException.ThrowIf(currency.Length != OrderConstants.CurrencyLength, $"Currency must be {OrderConstants.CurrencyLength} characters long.");

        UserId = userId;
        Currency = currency.ToUpperInvariant();
        Status = OrderStatus.Placed;
        Items = [];
    }

    public void AddItems(IEnumerable<OrderItem> items)
    {
        DomainException.ThrowIf(items == null || !items.Any(), "Order must have at least one item.");
        Items.AddRange(items!);
        RecalculateTotal();
        UpdateTimestamps();
    }

    private Order()
    {
    }

    public bool CanConfirm() => Status is OrderStatus.Placed or OrderStatus.Confirmed;

    public bool TryConfirm()
    {
        if (Status == OrderStatus.Confirmed)
        {
            return true;
        }

        if (Status != OrderStatus.Placed)
        {
            return false;
        }

        Status = OrderStatus.Confirmed;
        UpdateTimestamps();
        return true;
    }

    public bool CanCancel() => Status is OrderStatus.Placed or OrderStatus.Confirmed or OrderStatus.Canceled;

    public bool TryCancel()
    {
        if (Status == OrderStatus.Canceled)
        {
            return true;
        }

        if (Status is not (OrderStatus.Placed or OrderStatus.Confirmed))
        {
            return false;
        }

        Status = OrderStatus.Canceled;
        UpdateTimestamps();
        return true;
    }

    public void RecalculateTotal() => Total = Items.Sum(i => i.Subtotal);
}