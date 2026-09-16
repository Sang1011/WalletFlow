using MediatR;
using WalletFlow.Application.Auth.Common;
using WalletFlow.Application.Common.Constants;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IWalletSettingsProvider _walletSettings;

    public RegisterCommandHandler(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IWalletSettingsProvider walletSettings)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _walletSettings = walletSettings;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var usernameExists = _dbContext.Users.Any(u => u.Username == request.Username);
        if (usernameExists)
            return Result<AuthResponse>.Failure("Tên đăng nhập đã tồn tại.", ErrorCodes.Auth.UsernameExists);

        var phoneExists = _dbContext.Users.Any(u => u.PhoneNumber == request.PhoneNumber);
        if (phoneExists)
            return Result<AuthResponse>.Failure("Số điện thoại đã được sử dụng.", ErrorCodes.Auth.PhoneExists);

        var emailExists = _dbContext.Users.Any(u => u.Email == request.Email);
        if (emailExists)
            return Result<AuthResponse>.Failure("Email đã được sử dụng.", ErrorCodes.Auth.EmailExists);

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Username, request.PhoneNumber, request.Email, passwordHash, request.FullName);

        var defaultWallet = Wallet.Create(user.Id, _walletSettings.GetDefaultCurrency());

        _dbContext.Users.Add(user);
        _dbContext.Wallets.Add(defaultWallet);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, _tokenService.GetRefreshTokenExpiry());

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(
            user.Id,
            user.Username,
            user.PhoneNumber,
            user.Email,
            user.FullName,
            accessToken,
            refreshTokenValue);

        return Result<AuthResponse>.Success(response);
    }
}