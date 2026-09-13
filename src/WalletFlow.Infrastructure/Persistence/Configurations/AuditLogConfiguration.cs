using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityName).IsRequired().HasMaxLength(100);

        builder.HasIndex(a => new { a.EntityName, a.EntityId });
    }
}