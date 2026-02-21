using Challange.Domain.Exceptions;

namespace Challange.Domain.Entities;

public class Product : BaseEntity
{
    public string Description { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int AvailableQuantity { get; private set; }

    public Product(string description, decimal unitPrice, int availableQuantity) : base()
    {
        DomainException.ThrowIf(string.IsNullOrWhiteSpace(description), "Description cannot be empty.");
        DomainException.ThrowIf(unitPrice <= 0, "Unit price must be greater than zero.");
        DomainException.ThrowIf(availableQuantity < 0, "Available quantity cannot be negative.");

        Description = description;
        UnitPrice = unitPrice;
        AvailableQuantity = availableQuantity;
    }

    public void DecreaseStock(int quantity)
    {
        DomainException.ThrowIf(quantity <= 0, "Quantity must be greater than zero.");
        DomainException.ThrowIf(quantity > AvailableQuantity, "Insufficient stock.");

        AvailableQuantity -= quantity;
        UpdateTimestamps();
    }

    public void IncreaseStock(int quantity)
    {
        DomainException.ThrowIf(quantity <= 0, "Quantity must be greater than zero.");

        AvailableQuantity += quantity;
        UpdateTimestamps();
    }

    private Product()
    { }
}