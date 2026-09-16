using MediatR;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Transactions.Common;

namespace WalletFlow.Application.Transactions.Queries.GetTransactionHistory;

public record GetTransactionHistoryQuery(
    Guid WalletId,
    int PageNumber = 1,
    int PageSize = 20,
    string? Type = null,
    string? Status = null) : IRequest<Result<PaginatedList<TransactionResponse>>>;