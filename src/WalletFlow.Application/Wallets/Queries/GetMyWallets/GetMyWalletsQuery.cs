using MediatR;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Wallets.Common;

namespace WalletFlow.Application.Wallets.Queries.GetMyWallets;

public record GetMyWalletsQuery : IRequest<Result<List<WalletResponse>>>;