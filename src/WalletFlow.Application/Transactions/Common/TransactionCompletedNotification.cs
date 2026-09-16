namespace WalletFlow.Application.Transactions.Common;

public record TransactionCompletedNotification(Guid TransactionId, Guid WalletId, string Type, decimal Amount, string Currency);