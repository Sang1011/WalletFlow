using WalletFlow.Domain.Common;

namespace WalletFlow.Domain.Entities;

public class ReconciliationMismatch : BaseEntity
{
    public Guid WalletId { get; private set; }
    public decimal ActualBalance { get; private set; }
    public decimal ExpectedBalanceFromLedger { get; private set; }
    public decimal Difference { get; private set; }
    public DateTime DetectedAtUtc { get; private set; }

    private ReconciliationMismatch() { }

    public static ReconciliationMismatch Create(Guid walletId, decimal actualBalance, decimal expectedBalance)
    {
        return new ReconciliationMismatch
        {
            WalletId = walletId,
            ActualBalance = actualBalance,
            ExpectedBalanceFromLedger = expectedBalance,
            Difference = actualBalance - expectedBalance,
            DetectedAtUtc = DateTime.UtcNow
        };
    }
}