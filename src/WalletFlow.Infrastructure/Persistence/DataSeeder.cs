using Microsoft.Extensions.Logging;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Domain.Entities;
using WalletFlow.Domain.Enums;
using WalletFlow.Domain.ValueObjects;

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

        var defaultCurrency = walletSettings.GetDefaultCurrency(); // VND

        // ---- Users ----
        var adminPasswordHash = passwordHasher.Hash("Admin@123");
        var admin = User.Create("admin", "0900000001", "admin@walletflow.local", adminPasswordHash, "Quản Trị Viên");
        admin.PromoteToAdmin();

        var userPasswordHash = passwordHasher.Hash("User@123");
        var user = User.Create("testuser", "0900000002", "user@walletflow.local", userPasswordHash, "Người Dùng Test");

        dbContext.Users.AddRange(admin, user);

        // ---- Wallets ----
        var adminWallet = Wallet.Create(admin.Id, defaultCurrency);       // VND
        var userWalletVnd = Wallet.Create(user.Id, defaultCurrency);      // VND
        var userWalletUsd = Wallet.Create(user.Id, CurrencyCode.USD);     // USD — để test currency mismatch, đa ví

        dbContext.Wallets.AddRange(adminWallet, userWalletVnd, userWalletUsd);

        // ---- Sample Transactions ----

        // 1. Deposit 1,000,000 VND vào ví testuser
        var depositAmount = Money.Create(1_000_000m, defaultCurrency);
        userWalletVnd.Credit(depositAmount);

        var depositTx = Transaction.CreatePending(
            TransactionType.Deposit, depositAmount.Amount, defaultCurrency,
            sourceWalletId: null, destinationWalletId: userWalletVnd.Id,
            idempotencyKey: "seed-deposit-testuser");
        depositTx.MarkSuccess();

        var depositLedger = LedgerEntry.Create(
            depositTx.Id, userWalletVnd.Id, LedgerEntryType.Credit,
            depositAmount.Amount, defaultCurrency, userWalletVnd.Balance);
        depositTx.AddLedgerEntry(depositLedger);

        // 2. Withdraw 200,000 VND từ ví testuser
        var withdrawAmount = Money.Create(200_000m, defaultCurrency);
        userWalletVnd.Debit(withdrawAmount);

        var withdrawTx = Transaction.CreatePending(
            TransactionType.Withdraw, withdrawAmount.Amount, defaultCurrency,
            sourceWalletId: userWalletVnd.Id, destinationWalletId: null,
            idempotencyKey: "seed-withdraw-testuser");
        withdrawTx.MarkSuccess();

        var withdrawLedger = LedgerEntry.Create(
            withdrawTx.Id, userWalletVnd.Id, LedgerEntryType.Debit,
            withdrawAmount.Amount, defaultCurrency, userWalletVnd.Balance);
        withdrawTx.AddLedgerEntry(withdrawLedger);

        // 3. Transfer 100,000 VND từ testuser -> admin
        var transferAmount = Money.Create(100_000m, defaultCurrency);
        userWalletVnd.Debit(transferAmount);
        adminWallet.Credit(transferAmount);

        var transferTx = Transaction.CreatePending(
            TransactionType.TransferOut, transferAmount.Amount, defaultCurrency,
            sourceWalletId: userWalletVnd.Id, destinationWalletId: adminWallet.Id,
            idempotencyKey: "seed-transfer-testuser-to-admin");
        transferTx.MarkSuccess();

        var transferDebitLedger = LedgerEntry.Create(
            transferTx.Id, userWalletVnd.Id, LedgerEntryType.Debit,
            transferAmount.Amount, defaultCurrency, userWalletVnd.Balance);

        var transferCreditLedger = LedgerEntry.Create(
            transferTx.Id, adminWallet.Id, LedgerEntryType.Credit,
            transferAmount.Amount, defaultCurrency, adminWallet.Balance);

        transferTx.AddLedgerEntry(transferDebitLedger);
        transferTx.AddLedgerEntry(transferCreditLedger);

        dbContext.Transactions.AddRange(depositTx, withdrawTx, transferTx);
        dbContext.LedgerEntries.AddRange(depositLedger, withdrawLedger, transferDebitLedger, transferCreditLedger);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seed dữ liệu thành công: 1 Admin, 1 User, 3 Wallets, 3 Transactions.");
        logger.LogInformation("Admin    -> username: admin    | password: Admin@123 | Wallet VND balance: {Balance}", adminWallet.Balance);
        logger.LogInformation("TestUser -> username: testuser | password: User@123  | Wallet VND balance: {Balance} | Wallet USD balance: {UsdBalance}",
            userWalletVnd.Balance, userWalletUsd.Balance);
    }
}