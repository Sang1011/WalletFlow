using Microsoft.Extensions.Options;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Domain.Enums;
using WalletFlow.Infrastructure.Settings;

namespace WalletFlow.Infrastructure.Services;

public class WalletSettingsProvider : IWalletSettingsProvider
{
    private readonly WalletSettings _settings;

    public WalletSettingsProvider(IOptions<WalletSettings> options)
    {
        _settings = options.Value;
    }

    public CurrencyCode GetDefaultCurrency()
    {
        if (!Enum.TryParse<CurrencyCode>(_settings.DefaultCurrency, ignoreCase: true, out var currency))
            return CurrencyCode.VND;

        return currency;
    }
}