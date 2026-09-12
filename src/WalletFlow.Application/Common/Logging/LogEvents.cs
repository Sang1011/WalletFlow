namespace WalletFlow.Application.Common.Logging;

// Đánh số theo khu vực để tránh trùng EventId: 1xxx = Auth, 2xxx = Wallet, 3xxx = Transaction, 9xxx = Common
public static class LogEvents
{
    public const int RequestStarted = 9000;
    public const int RequestCompleted = 9001;
    public const int RequestFailed = 9002;
    public const int ValidationFailed = 9003;

    public const int WalletCreated = 2000;
    public const int WalletLocked = 2001;
    public const int InsufficientBalance = 2002;
    public const int ConcurrencyConflict = 2003;

    public const int TransactionCreated = 3000;
    public const int TransactionSucceeded = 3001;
    public const int TransactionFailed = 3002;
    public const int TransactionReversed = 3003;
    public const int IdempotentRequestSkipped = 3004;
}