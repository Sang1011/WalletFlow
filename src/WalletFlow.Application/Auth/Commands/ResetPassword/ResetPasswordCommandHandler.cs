using MediatR;
using WalletFlow.Application.Common.Constants;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IAppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditLogService _auditLogService;
    private readonly ICacheService _cacheService;

    public ResetPasswordCommandHandler(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IAuditLogService auditLogService,
        ICacheService cacheService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _auditLogService = auditLogService;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
        if (user is null)
            return Result.Failure("Thông tin không hợp lệ.", ErrorCodes.Auth.InvalidRequest);

        var otpKey = BuildOtpKey(request.PhoneNumber);
        var cachedOtp = await _cacheService.GetAsync<string>(otpKey, cancellationToken);

        if (cachedOtp is null || cachedOtp != request.OtpCode)
            return Result.Failure("Mã OTP không hợp lệ hoặc đã hết hạn.", ErrorCodes.Auth.InvalidOtp);

        await _cacheService.RemoveAsync(otpKey, cancellationToken);

        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newPasswordHash);

        _auditLogService.Log(
            action: "RESET_PASSWORD",
            entityName: nameof(user),
            entityId: user.Id,
            oldValue: null,
            newValue: null);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static string BuildOtpKey(string phoneNumber) => $"otp:reset-password:{phoneNumber}";
}