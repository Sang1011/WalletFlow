using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => rt.UserId);
    }
}