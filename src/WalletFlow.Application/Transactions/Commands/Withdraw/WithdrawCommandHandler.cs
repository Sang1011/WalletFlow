using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletFlow.Application.Common.Constants;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Logging;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Transactions.Common;
using WalletFlow.Domain.Entities;
using WalletFlow.Domain.Enums;
using WalletFlow.Domain.Exceptions;
using WalletFlow.Domain.ValueObjects;

namespace WalletFlow.Application.Transactions.Commands.Withdraw;

public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, Result<TransactionResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<WithdrawCommandHandler> _logger;

    public WithdrawCommandHandler(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService,
        IAuditLogService auditLogService,
        ILogger<WithdrawCommandHandler> logger)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<Result<TransactionResponse>> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var wallet = _dbContext.Wallets.FirstOrDefault(w => w.Id == request.WalletId);
        if (wallet is null || wallet.UserId != userId)
            return Result<TransactionResponse>.Failure("Ví không tồn tại.", ErrorCodes.Wallet.NotFound);

        var balanceBefore = wallet.Balance;

        var transaction = Transaction.CreatePending(
            TransactionType.Withdraw, request.Amount, wallet.Currency,
            wallet.Id, null, request.IdempotencyKey);

        try
        {
            var money = Money.Create(request.Amount, wallet.Currency);
            wallet.Debit(money);

            var ledgerEntry = LedgerEntry.Create(
                transaction.Id, wallet.Id, LedgerEntryType.Debit,
                request.Amount, wallet.Currency, wallet.Balance);

            transaction.AddLedgerEntry(ledgerEntry);
            transaction.MarkSuccess();

            _dbContext.Transactions.Add(transaction);
            _dbContext.LedgerEntries.Add(ledgerEntry);


            _auditLogService.Log(
                action: "WITHDRAW",
                entityName: nameof(Wallet),
                entityId: wallet.Id,
                oldValue: new { Balance = balanceBefore },
                newValue: new { Balance = wallet.Balance, TransactionId = transaction.Id });

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.TransactionCreated(transaction.Id, transaction.Type.ToString(), transaction.Amount);
            _logger.TransactionSucceeded(transaction.Id);

            return Result<TransactionResponse>.Success(transaction.ToResponse());
        }
        catch (InsufficientBalanceException)
        {
            _logger.InsufficientBalance(wallet.Id, request.Amount);
            return Result<TransactionResponse>.Failure("Số dư không đủ để thực hiện giao dịch.", ErrorCodes.Transaction.InsufficientBalance);
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.ConcurrencyConflict(wallet.Id, 1);
            return Result<TransactionResponse>.Failure(
                "Ví đang được xử lý bởi giao dịch khác, vui lòng thử lại.", ErrorCodes.Wallet.ConcurrencyConflict);
        }
    }
}