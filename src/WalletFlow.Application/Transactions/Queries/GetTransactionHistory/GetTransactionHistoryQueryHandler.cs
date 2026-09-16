using MediatR;
using WalletFlow.Application.Common.Constants;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Transactions.Common;
using WalletFlow.Domain.Enums;

namespace WalletFlow.Application.Transactions.Queries.GetTransactionHistory;

public class GetTransactionHistoryQueryHandler
    : IRequestHandler<GetTransactionHistoryQuery, Result<PaginatedList<TransactionResponse>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetTransactionHistoryQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public Task<Result<PaginatedList<TransactionResponse>>> Handle(
        GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var wallet = _dbContext.Wallets.FirstOrDefault(w => w.Id == request.WalletId);
        if (wallet is null || wallet.UserId != userId)
            return Task.FromResult(Result<PaginatedList<TransactionResponse>>.Failure(
                "Ví không tồn tại.", ErrorCodes.Wallet.NotFound));

        var query = _dbContext.Transactions
            .Where(t => t.SourceWalletId == request.WalletId || t.DestinationWalletId == request.WalletId);

        if (!string.IsNullOrWhiteSpace(request.Type) &&
            Enum.TryParse<TransactionType>(request.Type, true, out var type))
        {
            query = query.Where(t => t.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<TransactionStatus>(request.Status, true, out var status))
        {
            query = query.Where(t => t.Status == status);
        }

        var totalCount = query.Count();

        var transactions = query
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList()
            .ToResponseList();

        var result = new PaginatedList<TransactionResponse>(
            transactions, totalCount, request.PageNumber, request.PageSize);

        return Task.FromResult(Result<PaginatedList<TransactionResponse>>.Success(result));
    }
}