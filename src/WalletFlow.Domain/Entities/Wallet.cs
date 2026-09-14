using WalletFlow.Domain.Common;
using WalletFlow.Domain.Enums;
using WalletFlow.Domain.Exceptions;
using WalletFlow.Domain.ValueObjects;

namespace WalletFlow.Domain.Entities;

public class Wallet : BaseEntity
{
    public Guid UserId { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public decimal Balance { get; private set; }
    public WalletStatus Status { get; private set; }

    public uint Version { get; private set; }

    private Wallet() { }

    public static Wallet Create(Guid userId, CurrencyCode currency)
    {
        return new Wallet
        {
            UserId = userId,
            Currency = currency,
            Balance = 0,
            Status = WalletStatus.Active
        };
    }

    public Money GetBalance() => Money.Create(Balance, Currency);

    public void Credit(Money amount)
    {
        EnsureActive();
        EnsureSameCurrency(amount);
        Balance += amount.Amount;
        MarkUpdated();
    }

    public void Debit(Money amount)
    {
        EnsureActive();
        EnsureSameCurrency(amount);
        if (Balance < amount.Amount)
            throw new InsufficientBalanceException();

        Balance -= amount.Amount;
        MarkUpdated();
    }

    public void Lock() => Status = WalletStatus.Locked;
    public void Unlock() => Status = WalletStatus.Active;

    private void EnsureActive()
    {
        if (Status != WalletStatus.Active)
            throw new DomainException("Ví hiện không hoạt động (đang bị khoá hoặc đã đóng).");
    }

    private void EnsureSameCurrency(Money amount)
    {
        if (amount.Currency != Currency)
            throw new DomainException("Loại tiền tệ không khớp với ví.");
    }
}