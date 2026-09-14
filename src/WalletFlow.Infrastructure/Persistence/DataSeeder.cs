using Microsoft.Extensions.Logging;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Domain.Entities;
using WalletFlow.Domain.Enums;

namespace WalletFlow.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(
    AppDbContext dbContext,
    IPasswordHasher passwordHasher,
    IWalletSettingsProvider walletSettings,
    ILogger logger)
    {
        if (dbContext.Users.Any())
        {
            logger.LogInformation("Database đã có dữ liệu, bỏ qua seed.");
            return;
        }

        var defaultCurrency = walletSettings.GetDefaultCurrency();

        var adminPasswordHash = passwordHasher.Hash("Admin@123");
        var admin = User.Create("admin", "0900000001", "admin@walletflow.local", adminPasswordHash, "Quản Trị Viên");
        admin.PromoteToAdmin();
        var adminWallet = Wallet.Create(admin.Id, defaultCurrency);

        var userPasswordHash = passwordHasher.Hash("User@123");
        var user = User.Create("testuser", "0900000002", "user@walletflow.local", userPasswordHash, "Người Dùng Test");
        var userWallet = Wallet.Create(user.Id, defaultCurrency);

        dbContext.Users.AddRange(admin, user);
        dbContext.Wallets.AddRange(adminWallet, userWallet);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seed dữ liệu thành công: 1 Admin, 1 User.");
        logger.LogInformation("Admin -> username: admin | password: Admin@123");
        logger.LogInformation("User  -> username: testuser | password: User@123");
    }
}