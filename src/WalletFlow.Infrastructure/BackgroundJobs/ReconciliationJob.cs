using Microsoft.Extensions.Logging;
using WalletFlow.Domain.Entities;
using WalletFlow.Domain.Enums;
using WalletFlow.Infrastructure.Persistence;

namespace WalletFlow.Infrastructure.BackgroundJobs;

public class ReconciliationJob
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ReconciliationJob> _logger;

    public ReconciliationJob(AppDbContext dbContext, ILogger<ReconciliationJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Reconciliation: bắt đầu đối soát toàn bộ ví.");

        var wallets = _dbContext.Wallets.ToList();
        var mismatches = new List<ReconciliationMismatch>();

        foreach (var wallet in wallets)
        {
            var ledgerEntries = _dbContext.LedgerEntries.Where(le => le.WalletId == wallet.Id).ToList();

            var totalCredit = ledgerEntries.Where(le => le.EntryType == LedgerEntryType.Credit).Sum(le => le.Amount);
            var totalDebit = ledgerEntries.Where(le => le.EntryType == LedgerEntryType.Debit).Sum(le => le.Amount);
            var expectedBalance = totalCredit - totalDebit;

            if (wallet.Balance != expectedBalance)
            {
                var mismatch = ReconciliationMismatch.Create(wallet.Id, wallet.Balance, expectedBalance);
                mismatches.Add(mismatch);

                _logger.LogWarning(
                    "Reconciliation: PHÁT HIỆN LỆCH ví {WalletId} — Balance thực tế: {Actual}, Balance tính từ Ledger: {Expected}, Chênh lệch: {Diff}",
                    wallet.Id, wallet.Balance, expectedBalance, mismatch.Difference);
            }
        }

        if (mismatches.Count > 0)
        {
            _dbContext.ReconciliationMismatches.AddRange(mismatches);
            await _dbContext.SaveChangesAsync();

            _logger.LogWarning("Reconciliation: hoàn tất — phát hiện {Count} ví bị lệch số dư.", mismatches.Count);
        }
        else
        {
            _logger.LogInformation("Reconciliation: hoàn tất — {Count} ví đã kiểm tra, không phát hiện lệch.", wallets.Count);
        }
    }
}