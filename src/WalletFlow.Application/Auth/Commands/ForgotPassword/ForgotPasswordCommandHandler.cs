using MediatR;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IAppDbContext _dbContext;
    private readonly ISmsSender _smsSender;

    public ForgotPasswordCommandHandler(IAppDbContext dbContext, ISmsSender smsSender)
    {
        _dbContext = dbContext;
        _smsSender = smsSender;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);

        // Luôn trả Success dù số điện thoại không tồn tại — tránh lộ thông tin
        // "số này có đăng ký hay không" cho kẻ tấn công dò số (user enumeration).
        if (user is null)
            return Result.Success();

        var otpCode = GenerateOtp();
        var otp = PasswordResetOtp.Create(user.Id, otpCode, DateTime.UtcNow.AddMinutes(5));

        _dbContext.PasswordResetOtps.Add(otp);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _smsSender.SendOtpAsync(request.PhoneNumber, otpCode, cancellationToken);

        return Result.Success();
    }

    private static string GenerateOtp() => Random.Shared.Next(100000, 999999).ToString();
}