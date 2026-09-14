using MediatR;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IAppDbContext _dbContext;
    private readonly ISmsSender _smsSender;
    private readonly ICacheService _cacheService;

    private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(10);

    public ForgotPasswordCommandHandler(
        IAppDbContext dbContext,
        ISmsSender smsSender,
        ICacheService cacheService)
    {
        _dbContext = dbContext;
        _smsSender = smsSender;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);

        if (user is null)
            return Result.Success();

        var otpCode = GenerateOtp();

        await _cacheService.SetAsync(
            BuildOtpKey(request.PhoneNumber),
            otpCode,
            OtpTtl,
            cancellationToken);

        await _smsSender.SendOtpAsync(request.PhoneNumber, otpCode, cancellationToken);

        return Result.Success();
    }

    private static string GenerateOtp() => Random.Shared.Next(100000, 999999).ToString();

    private static string BuildOtpKey(string phoneNumber) => $"otp:reset-password:{phoneNumber}";
}