using WalletFlow.Domain.Common;

namespace WalletFlow.Domain.Entities;

public class PasswordResetOtp : BaseEntity
{
    public Guid UserId { get; private set; }
    public string OtpCode { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsUsed { get; private set; }

    private PasswordResetOtp() { }

    public static PasswordResetOtp Create(Guid userId, string otpCode, DateTime expiresAtUtc)
    {
        return new PasswordResetOtp
        {
            UserId = userId,
            OtpCode = otpCode,
            ExpiresAtUtc = expiresAtUtc,
            IsUsed = false
        };
    }

    public bool IsValid => !IsUsed && ExpiresAtUtc > DateTime.UtcNow;

    public void MarkUsed() => IsUsed = true;
}