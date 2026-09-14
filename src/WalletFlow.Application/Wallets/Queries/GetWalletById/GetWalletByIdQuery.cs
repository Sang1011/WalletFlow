using MediatR;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Wallets.Common;

namespace WalletFlow.Application.Wallets.Queries.GetWalletById;

public record GetWalletByIdQuery(Guid WalletId) : IRequest<Result<WalletResponse>>;