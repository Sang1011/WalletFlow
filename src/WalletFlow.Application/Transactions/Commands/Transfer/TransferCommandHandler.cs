using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Logging;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Transactions.Common;
using WalletFlow.Domain.Entities;
using WalletFlow.Domain.Enums;
using WalletFlow.Domain.Exceptions;
using WalletFlow.Domain.ValueObjects;
using WalletFlow.Application.Common.Constants;

namespace WalletFlow.Application.Transactions.Commands.Transfer;

public class TransferCommandHandler : IRequestHandler<TransferCommand, Result<TransactionResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<TransferCommandHandler> _logger;

    public TransferCommandHandler(
        IAppDbContext dbContext,
        ICurrentUserService currentUserService,
        IAuditLogService auditLogService,
        ILogger<TransferCommandHandler> logger)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<Result<TransactionResponse>> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var sourceWallet = _dbContext.Wallets.FirstOrDefault(w => w.Id == request.SourceWalletId);
        if (sourceWallet is null || sourceWallet.UserId != userId)
            return Result<TransactionResponse>.Failure("Ví nguồn không tồn tại.", ErrorCodes.Transaction.SourceWalletNotFound);

        var destinationWallet = _dbContext.Wallets.FirstOrDefault(w => w.Id == request.DestinationWalletId);
        if (destinationWallet is null)
            return Result<TransactionResponse>.Failure("Ví đích không tồn tại.", ErrorCodes.Transaction.DestinationWalletNotFound);

        if (destinationWallet.Currency != sourceWallet.Currency)
            return Result<TransactionResponse>.Failure(
                "Không thể chuyển tiền giữa hai ví khác loại tiền tệ.", ErrorCodes.Transaction.CurrencyMismatch);

        var sourceBalanceBefore = sourceWallet.Balance;
        var destBalanceBefore = destinationWallet.Balance;

        var transaction = Transaction.CreatePending(
            TransactionType.TransferOut,
            request.Amount,
            sourceWallet.Currency,
            sourceWallet.Id,
            destinationWallet.Id,
            request.IdempotencyKey);

        try
        {
            var money = Money.Create(request.Amount, sourceWallet.Currency);

            sourceWallet.Debit(money);
            destinationWallet.Credit(money);

            var debitEntry = LedgerEntry.Create(
                transaction.Id, sourceWallet.Id, LedgerEntryType.Debit,
                request.Amount, sourceWallet.Currency, sourceWallet.Balance);

            var creditEntry = LedgerEntry.Create(
                transaction.Id, destinationWallet.Id, LedgerEntryType.Credit,
                request.Amount, destinationWallet.Currency, destinationWallet.Balance);

            transaction.AddLedgerEntry(debitEntry);
            transaction.AddLedgerEntry(creditEntry);
            transaction.MarkSuccess();

            _dbContext.Transactions.Add(transaction);
            _dbContext.LedgerEntries.AddRange(debitEntry, creditEntry);

            _auditLogService.Log(
                action: "TRANSFER_OUT",
                entityName: nameof(Wallet),
                entityId: sourceWallet.Id,
                oldValue: new { Balance = sourceBalanceBefore },
                newValue: new { Balance = sourceWallet.Balance, TransactionId = transaction.Id, ToWalletId = destinationWallet.Id });

            _auditLogService.Log(
                action: "TRANSFER_IN",
                entityName: nameof(Wallet),
                entityId: destinationWallet.Id,
                oldValue: new { Balance = destBalanceBefore },
                newValue: new { Balance = destinationWallet.Balance, TransactionId = transaction.Id, FromWalletId = sourceWallet.Id });

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.TransactionCreated(transaction.Id, transaction.Type.ToString(), transaction.Amount);
            _logger.TransactionSucceeded(transaction.Id);

            return Result<TransactionResponse>.Success(transaction.ToResponse());
        }
        catch (InsufficientBalanceException)
        {
            _logger.InsufficientBalance(sourceWallet.Id, request.Amount);
            return Result<TransactionResponse>.Failure("Số dư không đủ để thực hiện giao dịch.", ErrorCodes.Transaction.InsufficientBalance);
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.ConcurrencyConflict(sourceWallet.Id, 1);
            return Result<TransactionResponse>.Failure(
                "Một trong hai ví đang được xử lý bởi giao dịch khác, vui lòng thử lại.", ErrorCodes.Wallet.ConcurrencyConflict);
        }
    }
}