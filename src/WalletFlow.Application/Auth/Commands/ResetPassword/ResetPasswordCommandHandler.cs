using MediatR;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IAppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(IAppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
        if (user is null)
            return Result.Failure("Thông tin không hợp lệ.", "INVALID_REQUEST");

        var otp = _dbContext.PasswordResetOtps
            .Where(o => o.UserId == user.Id && o.OtpCode == request.OtpCode)
            .OrderByDescending(o => o.CreatedAtUtc)
            .FirstOrDefault();

        if (otp is null || !otp.IsValid)
            return Result.Failure("Mã OTP không hợp lệ hoặc đã hết hạn.", "INVALID_OTP");

        otp.MarkUsed();

        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newPasswordHash);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}