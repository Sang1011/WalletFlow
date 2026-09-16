using MediatR;
using Microsoft.EntityFrameworkCore;
using WalletFlow.Application.Common.Constants;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Logging;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Transactions.Common;
using WalletFlow.Domain.Entities;
using WalletFlow.Domain.Enums;
using WalletFlow.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace WalletFlow.Application.Transactions.Commands.Deposit;

public class DepositCommandHandler : IRequestHandler<DepositCommand, Result<TransactionResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLogService;
    private readonly IOutboxService _outboxService;
    private readonly ILogger<DepositCommandHandler> _logger;

    public DepositCommandHandler(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService,
        IAuditLogService auditLogService,
        IOutboxService outboxService,
        ILogger<DepositCommandHandler> logger)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _auditLogService = auditLogService;
        _outboxService = outboxService;
        _logger = logger;
    }

    public async Task<Result<TransactionResponse>> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var wallet = _dbContext.Wallets.FirstOrDefault(w => w.Id == request.WalletId);
        if (wallet is null || wallet.UserId != userId)
            return Result<TransactionResponse>.Failure("Ví không tồn tại.", ErrorCodes.Wallet.NotFound);

        var balanceBefore = wallet.Balance;

        var transaction = Transaction.CreatePending(
            TransactionType.Deposit,
            request.Amount,
            wallet.Currency,
            sourceWalletId: null,
            destinationWalletId: wallet.Id,
            request.IdempotencyKey);

        try
        {
            var money = Money.Create(request.Amount, wallet.Currency);
            wallet.Credit(money);

            var ledgerEntry = LedgerEntry.Create(
                transaction.Id, wallet.Id, LedgerEntryType.Credit,
                request.Amount, wallet.Currency, wallet.Balance);

            transaction.AddLedgerEntry(ledgerEntry);
            transaction.MarkSuccess();

            _dbContext.Transactions.Add(transaction);
            _dbContext.LedgerEntries.Add(ledgerEntry);

            _auditLogService.Log(
                action: "DEPOSIT",
                entityName: nameof(Wallet),
                entityId: wallet.Id,
                oldValue: new { Balance = balanceBefore },
                newValue: new { Balance = wallet.Balance, TransactionId = transaction.Id });

            _outboxService.Enqueue(
                type: "TransactionCompleted",
                payload: new TransactionCompletedNotification(transaction.Id, wallet.Id, "Deposit", request.Amount, wallet.Currency.ToString()));

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.TransactionCreated(transaction.Id, transaction.Type.ToString(), transaction.Amount);
            _logger.TransactionSucceeded(transaction.Id);

            return Result<TransactionResponse>.Success(transaction.ToResponse());
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.ConcurrencyConflict(wallet.Id, 1);
            return Result<TransactionResponse>.Failure(
                "Ví đang được xử lý bởi giao dịch khác, vui lòng thử lại.", ErrorCodes.Wallet.ConcurrencyConflict);
        }
    }
}