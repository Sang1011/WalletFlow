using WalletFlow.Domain.Entities;

namespace WalletFlow.Application.Wallets.Common;

public record WalletResponse(
    Guid Id,
    string Currency,
    decimal Balance,
    string Status,
    DateTime CreatedAtUtc);

public static class WalletMappingExtensions
{
    public static WalletResponse ToResponse(this Wallet wallet)
    {
        return new WalletResponse(
            wallet.Id,
            wallet.Currency.ToString(),
            wallet.Balance,
            wallet.Status.ToString(),
            wallet.CreatedAtUtc);
    }

    public static List<WalletResponse> ToResponseList(this IEnumerable<Wallet> wallets)
    {
        return wallets.Select(w => w.ToResponse()).ToList();
    }
}