namespace Challenge.Application.Features.Shared.Products;

public record ProductResponse(Guid Id, string Description, decimal UnitPrice, int AvailableQuantity);