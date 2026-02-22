using Challange.Domain.Constants;
using Challange.Domain.Exceptions;

namespace Challange.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Subtotal => UnitPrice * Quantity;

    public OrderItem(Guid orderId, Guid productId, decimal unitPrice, int quantity) : base()
    {
        DomainException.ThrowIf(orderId == Guid.Empty, "Order ID cannot be empty.");
        DomainException.ThrowIf(productId == Guid.Empty, "Product ID cannot be empty.");
        DomainException.ThrowIf(unitPrice < OrderItemConstants.MinUnitPrice, $"Unit price must be at least {OrderItemConstants.MinUnitPrice}.");
        DomainException.ThrowIf(quantity < OrderItemConstants.MinQuantity, $"Quantity must be at least {OrderItemConstants.MinQuantity}.");

        OrderId = orderId;
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}