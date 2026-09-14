using MediatR;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Wallets.Common;

namespace WalletFlow.Application.Wallets.Queries.GetWalletById;

public class GetWalletByIdQueryHandler : IRequestHandler<GetWalletByIdQuery, Result<WalletResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetWalletByIdQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public Task<Result<WalletResponse>> Handle(GetWalletByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var wallet = _dbContext.Wallets.FirstOrDefault(w => w.Id == request.WalletId);

        if (wallet is null)
            return Task.FromResult(Result<WalletResponse>.Failure("Ví không tồn tại.", "WALLET_NOT_FOUND"));

        if (wallet.UserId != userId && !_currentUserService.IsAdmin)
            return Task.FromResult(Result<WalletResponse>.Failure("Ví không tồn tại.", "WALLET_NOT_FOUND"));

        return Task.FromResult(Result<WalletResponse>.Success(wallet.ToResponse()));
    }
}