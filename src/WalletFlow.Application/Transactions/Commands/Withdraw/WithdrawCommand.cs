using MediatR;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Transactions.Common;

namespace WalletFlow.Application.Transactions.Commands.Withdraw;

public record WithdrawCommand(Guid WalletId, decimal Amount, string IdempotencyKey)
    : IRequest<Result<TransactionResponse>>, IIdempotentRequest;