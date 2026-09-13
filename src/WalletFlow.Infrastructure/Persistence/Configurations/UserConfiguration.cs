using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15);
        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);

        builder.HasMany(u => u.Wallets)
            .WithOne()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}