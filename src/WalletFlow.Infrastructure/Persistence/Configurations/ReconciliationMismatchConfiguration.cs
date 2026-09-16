using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Infrastructure.Persistence.Configurations;

public class ReconciliationMismatchConfiguration : IEntityTypeConfiguration<ReconciliationMismatch>
{
    public void Configure(EntityTypeBuilder<ReconciliationMismatch> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.ActualBalance).HasPrecision(18, 2);
        builder.Property(m => m.ExpectedBalanceFromLedger).HasPrecision(18, 2);
        builder.Property(m => m.Difference).HasPrecision(18, 2);

        builder.HasIndex(m => m.WalletId);
    }
}