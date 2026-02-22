using Challange.Domain.Constants;
using Challange.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challange.Infrastructure.Persistence.Configurations;

internal class UserConfiguration : BaseEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.Login)
            .HasMaxLength(UserConstants.MaxLoginLength)
            .IsRequired();

        builder.Property(x => x.NormalizedLogin)
            .HasMaxLength(UserConstants.MaxLoginLength)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(UserConstants.MaxPasswordHashLength)
            .IsRequired();

        builder.HasIndex(x => x.NormalizedLogin).IsUnique();
    }
}