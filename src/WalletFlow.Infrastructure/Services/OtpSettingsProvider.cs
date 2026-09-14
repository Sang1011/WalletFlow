using Microsoft.Extensions.Options;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Infrastructure.Settings;

namespace WalletFlow.Infrastructure.Services;

public class OtpSettingsProvider : IOtpSettingsProvider
{
    private readonly OtpSettings _settings;

    public OtpSettingsProvider(IOptions<OtpSettings> options)
    {
        _settings = options.Value;
    }

    public TimeSpan GetOtpExpiry() => TimeSpan.FromMinutes(_settings.ExpiryMinutes);
}