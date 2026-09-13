using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(t => t.IdempotencyKey).HasMaxLength(100);

        builder.HasIndex(t => t.IdempotencyKey).IsUnique().HasFilter("\"IdempotencyKey\" IS NOT NULL");
        builder.HasIndex(t => t.SourceWalletId);
        builder.HasIndex(t => t.DestinationWalletId);

        builder.HasMany(t => t.LedgerEntries)
            .WithOne()
            .HasForeignKey(le => le.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}