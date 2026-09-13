using Microsoft.Extensions.Logging;
using WalletFlow.Application.Common.Interfaces;

namespace WalletFlow.Infrastructure.Services;

public class FakeSmsSender : ISmsSender
{
    private readonly ILogger<FakeSmsSender> _logger;

    public FakeSmsSender(ILogger<FakeSmsSender> logger)
    {
        _logger = logger;
    }

    public Task SendOtpAsync(string phoneNumber, string otpCode, CancellationToken cancellationToken = default)
    {
        // Giả lập gửi SMS — log ra console/log file thay vì gọi provider thật.
        _logger.LogInformation("[FAKE SMS] Gửi OTP {OtpCode} đến số {PhoneNumber}", otpCode, phoneNumber);
        return Task.CompletedTask;
    }
}