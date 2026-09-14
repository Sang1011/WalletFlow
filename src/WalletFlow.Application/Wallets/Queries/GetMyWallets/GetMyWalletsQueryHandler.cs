using MediatR;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Wallets.Common;

namespace WalletFlow.Application.Wallets.Queries.GetMyWallets;

public class GetMyWalletsQueryHandler : IRequestHandler<GetMyWalletsQuery, Result<List<WalletResponse>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetMyWalletsQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public Task<Result<List<WalletResponse>>> Handle(GetMyWalletsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var wallets = _dbContext.Wallets
            .Where(w => w.UserId == userId)
            .ToList()
            .ToResponseList();

        return Task.FromResult(Result<List<WalletResponse>>.Success(wallets));
    }
}