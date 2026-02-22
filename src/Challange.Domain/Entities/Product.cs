using Challange.Domain.Constants;
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
        DomainException.ThrowIf(description.Length > ProductConstants.MaxDescriptionLength, $"Description cannot exceed {ProductConstants.MaxDescriptionLength} characters.");
        DomainException.ThrowIf(unitPrice < ProductConstants.MinUnitPrice, $"Unit price must be at least {ProductConstants.MinUnitPrice}.");
        DomainException.ThrowIf(availableQuantity < ProductConstants.MinAvailableQuantity, $"Available quantity must be at least {ProductConstants.MinAvailableQuantity}.");

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