using MediatR;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Wallets.Common;
using WalletFlow.Domain.Enums;

namespace WalletFlow.Application.Wallets.Commands.CreateWallet;

public record CreateWalletCommand(CurrencyCode Currency) : IRequest<Result<WalletResponse>>;