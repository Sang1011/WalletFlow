using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Infrastructure.Persistence.Configurations;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.HasKey(le => le.Id);
        builder.Property(le => le.Amount).HasPrecision(18, 2);
        builder.Property(le => le.BalanceAfter).HasPrecision(18, 2);
        builder.Property(le => le.EntryType).HasConversion<string>().HasMaxLength(10);
        builder.Property(le => le.Currency).HasConversion<string>().HasMaxLength(10);

        builder.HasIndex(le => le.WalletId);
    }
}