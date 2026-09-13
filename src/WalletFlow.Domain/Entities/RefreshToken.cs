using WalletFlow.Domain.Common;

namespace WalletFlow.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAtUtc)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            IsRevoked = false
        };
    }

    public bool IsActive => !IsRevoked && ExpiresAtUtc > DateTime.UtcNow;

    public void Revoke() => IsRevoked = true;
}