using Microsoft.EntityFrameworkCore;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}