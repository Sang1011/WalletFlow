using WalletFlow.Domain.Common;
using WalletFlow.Domain.Enums;

namespace WalletFlow.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid? SourceWalletId { get; private set; }
    public Guid? DestinationWalletId { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public string? FailureReason { get; private set; }

    private readonly List<LedgerEntry> _ledgerEntries = new();
    public IReadOnlyCollection<LedgerEntry> LedgerEntries => _ledgerEntries.AsReadOnly();

    private Transaction() { }

    public static Transaction CreatePending(
        TransactionType type,
        decimal amount,
        CurrencyCode currency,
        Guid? sourceWalletId,
        Guid? destinationWalletId,
        string? idempotencyKey)
    {
        return new Transaction
        {
            Type = type,
            Status = TransactionStatus.Pending,
            Amount = amount,
            Currency = currency,
            SourceWalletId = sourceWalletId,
            DestinationWalletId = destinationWalletId,
            IdempotencyKey = idempotencyKey
        };
    }

    public void MarkSuccess()
    {
        Status = TransactionStatus.Success;
        MarkUpdated();
    }

    public void MarkFailed(string reason)
    {
        Status = TransactionStatus.Failed;
        FailureReason = reason;
        MarkUpdated();
    }

    public void MarkReversed()
    {
        Status = TransactionStatus.Reversed;
        MarkUpdated();
    }

    public void AddLedgerEntry(LedgerEntry entry) => _ledgerEntries.Add(entry);
}