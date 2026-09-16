using MediatR;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Transactions.Common;

namespace WalletFlow.Application.Transactions.Commands.Transfer;

public record TransferCommand(Guid SourceWalletId, Guid DestinationWalletId, decimal Amount, string IdempotencyKey)
    : IRequest<Result<TransactionResponse>>, IIdempotentRequest;