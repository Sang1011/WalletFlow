using WalletFlow.Domain.Enums;

namespace WalletFlow.Application.Common.Interfaces;

public interface IWalletSettingsProvider
{
    CurrencyCode GetDefaultCurrency();
}