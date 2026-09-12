using WalletFlow.Domain.Common;
using WalletFlow.Domain.Enums;

namespace WalletFlow.Domain.Entities;

public class LedgerEntry : BaseEntity
{
    public Guid TransactionId { get; private set; }
    public Guid WalletId { get; private set; }
    public LedgerEntryType EntryType { get; private set; }
    public decimal Amount { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public decimal BalanceAfter { get; private set; }

    private LedgerEntry() { }

    public static LedgerEntry Create(
        Guid transactionId,
        Guid walletId,
        LedgerEntryType entryType,
        decimal amount,
        CurrencyCode currency,
        decimal balanceAfter)
    {
        return new LedgerEntry
        {
            TransactionId = transactionId,
            WalletId = walletId,
            EntryType = entryType,
            Amount = amount,
            Currency = currency,
            BalanceAfter = balanceAfter
        };
    }
}