using Challange.Domain.Exceptions;

namespace Challange.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public OrderItem(Guid orderId, Guid productId, decimal unitPrice, int quantity) : base()
    {
        DomainException.ThrowIf(orderId == Guid.Empty, "Order ID cannot be empty.");
        DomainException.ThrowIf(productId == Guid.Empty, "Product ID cannot be empty.");
        DomainException.ThrowIf(unitPrice <= 0, "Unit price must be greater than zero.");
        DomainException.ThrowIf(quantity <= 0, "Quantity must be greater than zero.");

        OrderId = orderId;
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    private OrderItem()
    { }

    public decimal Subtotal => UnitPrice * Quantity;
}