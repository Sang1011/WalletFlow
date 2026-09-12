using WalletFlow.Domain.Common;

namespace WalletFlow.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public bool IsAdmin { get; private set; }
    public bool IsLocked { get; private set; }

    private readonly List<Wallet> _wallets = new();
    public IReadOnlyCollection<Wallet> Wallets => _wallets.AsReadOnly();

    private User() { }

    public static User Create(string email, string passwordHash, string fullName)
    {
        return new User
        {
            Email = email,
            PasswordHash = passwordHash,
            FullName = fullName,
            IsAdmin = false,
            IsLocked = false
        };
    }

    public void Lock() => IsLocked = true;
    public void Unlock() => IsLocked = false;
}