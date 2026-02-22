using Challange.Domain.Constants;
using Challange.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challange.Infrastructure.Persistence.Configurations;

internal class ProductConfiguration : BaseEntityConfiguration<Product>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.Property(x => x.Description)
            .HasColumnType("CITEXT")
            .HasMaxLength(ProductConstants.MaxDescriptionLength)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.AvailableQuantity)
            .IsRequired();

        builder.HasIndex(x => x.Description);
    }
}