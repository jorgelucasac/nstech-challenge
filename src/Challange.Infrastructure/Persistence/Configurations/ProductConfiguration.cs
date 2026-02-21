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
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.AvailableQuantity)
            .IsRequired();

        builder.HasData(
           new Product("Rice", 30m, 100),
           new Product("Beans", 5.90m, 200),
           new Product("Sugar", 10.50m, 500)
       );
    }
}