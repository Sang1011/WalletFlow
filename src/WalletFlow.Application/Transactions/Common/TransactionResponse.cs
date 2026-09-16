using WalletFlow.Domain.Entities;

namespace WalletFlow.Application.Transactions.Common;

public record TransactionResponse(
    Guid Id,
    string Type,
    string Status,
    decimal Amount,
    string Currency,
    Guid? SourceWalletId,
    Guid? DestinationWalletId,
    string? FailureReason,
    DateTime CreatedAtUtc);

public static class TransactionMappingExtensions
{
    public static TransactionResponse ToResponse(this Transaction transaction)
    {
        return new TransactionResponse(
            transaction.Id,
            transaction.Type.ToString(),
            transaction.Status.ToString(),
            transaction.Amount,
            transaction.Currency.ToString(),
            transaction.SourceWalletId,
            transaction.DestinationWalletId,
            transaction.FailureReason,
            transaction.CreatedAtUtc);
    }

    public static List<TransactionResponse> ToResponseList(this IEnumerable<Transaction> transactions) =>
        transactions.Select(t => t.ToResponse()).ToList();
}