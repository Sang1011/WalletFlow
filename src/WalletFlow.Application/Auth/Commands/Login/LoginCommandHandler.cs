using MediatR;
using WalletFlow.Application.Auth.Common;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(u =>
            u.Username == request.Identifier || u.PhoneNumber == request.Identifier);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Tên đăng nhập/SĐT hoặc mật khẩu không đúng.", "INVALID_CREDENTIALS");

        if (user.IsLocked)
            return Result<AuthResponse>.Failure("Tài khoản đã bị khoá.", "ACCOUNT_LOCKED");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, DateTime.UtcNow.AddDays(7));

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