using WalletFlow.Domain.Enums;
using WalletFlow.Domain.Exceptions;

namespace WalletFlow.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public CurrencyCode Currency { get; }

    private Money(decimal amount, CurrencyCode currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, CurrencyCode currency)
    {
        if (amount < 0)
            throw new DomainException("Số tiền không được âm.");

        return new Money(amount, currency);
    }

    public static Money Zero(CurrencyCode currency) => new(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        if (Amount < other.Amount)
            throw new InsufficientBalanceException();

        return new Money(Amount - other.Amount, Currency);
    }

    public bool IsGreaterOrEqual(Money other)
    {
        EnsureSameCurrency(other);
        return Amount >= other.Amount;
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Không thể thao tác giữa hai loại tiền tệ khác nhau.");
    }
}