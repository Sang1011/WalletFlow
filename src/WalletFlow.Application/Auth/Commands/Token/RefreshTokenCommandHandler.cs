using MediatR;
using WalletFlow.Application.Auth.Common;
using WalletFlow.Application.Common.Constants;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.Token;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IAppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = _dbContext.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (storedToken is null || !storedToken.IsActive)
            return Result<AuthResponse>.Failure("Refresh token không hợp lệ hoặc đã hết hạn.", ErrorCodes.Auth.InvalidRefreshToken);

        var user = _dbContext.Users.FirstOrDefault(u => u.Id == storedToken.UserId);
        if (user is null)
            return Result<AuthResponse>.Failure("Người dùng không tồn tại.", ErrorCodes.Auth.UserNotFound);

        storedToken.Revoke();

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();
        var newRefreshToken = Domain.Entities.RefreshToken.Create(
            user.Id,
            newRefreshTokenValue,
            _tokenService.GetRefreshTokenExpiry());

        _dbContext.RefreshTokens.Add(newRefreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(
            user.Id,
            user.Username,
            user.PhoneNumber,
            user.Email,
            user.FullName,
            newAccessToken,
            newRefreshTokenValue);

        return Result<AuthResponse>.Success(response);
    }
}