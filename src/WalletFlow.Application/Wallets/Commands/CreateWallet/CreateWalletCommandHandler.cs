using MediatR;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Wallets.Common;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Application.Wallets.Commands.CreateWallet;

public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, Result<WalletResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public CreateWalletCommandHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<WalletResponse>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var alreadyExists = _dbContext.Wallets.Any(w => w.UserId == userId && w.Currency == request.Currency);
        if (alreadyExists)
            return Result<WalletResponse>.Failure(
                $"Bạn đã có ví {request.Currency} rồi.", "WALLET_ALREADY_EXISTS");

        var wallet = Wallet.Create(userId, request.Currency);

        _dbContext.Wallets.Add(wallet);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<WalletResponse>.Success(wallet.ToResponse());
    }
}