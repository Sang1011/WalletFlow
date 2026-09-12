namespace WalletFlow.Domain.Exceptions;

public class InsufficientBalanceException : DomainException
{
    public InsufficientBalanceException()
        : base("Số dư không đủ để thực hiện giao dịch.") { }
}