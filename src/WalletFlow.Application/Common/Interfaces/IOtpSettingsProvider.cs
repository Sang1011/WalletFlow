namespace WalletFlow.Application.Common.Interfaces;

public interface IOtpSettingsProvider
{
    TimeSpan GetOtpExpiry();
}