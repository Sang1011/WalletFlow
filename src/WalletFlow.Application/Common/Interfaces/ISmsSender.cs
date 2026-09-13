namespace WalletFlow.Application.Common.Interfaces;

public interface ISmsSender
{
    Task SendOtpAsync(string phoneNumber, string otpCode, CancellationToken cancellationToken = default);
}