using Microsoft.Extensions.Logging;

namespace WalletFlow.Application.Common.Logging;

public static partial class AppLogMessages
{
    [LoggerMessage(LogEvents.RequestStarted, LogLevel.Information,
        "Bắt đầu xử lý request {RequestName} ({RequestGuid})")]
    public static partial void RequestStarted(this ILogger logger, string requestName, Guid requestGuid);

    [LoggerMessage(LogEvents.RequestCompleted, LogLevel.Information,
        "Hoàn tất request {RequestName} ({RequestGuid}) sau {ElapsedMs}ms")]
    public static partial void RequestCompleted(this ILogger logger, string requestName, Guid requestGuid, long elapsedMs);

    [LoggerMessage(LogEvents.RequestFailed, LogLevel.Error,
        "Request {RequestName} ({RequestGuid}) thất bại")]
    public static partial void RequestFailed(this ILogger logger, Exception ex, string requestName, Guid requestGuid);

    [LoggerMessage(LogEvents.ValidationFailed, LogLevel.Warning,
        "Validation thất bại cho {RequestName}: {Errors}")]
    public static partial void ValidationFailed(this ILogger logger, string requestName, string errors);

    [LoggerMessage(LogEvents.InsufficientBalance, LogLevel.Warning,
        "Ví {WalletId} không đủ số dư cho giao dịch {Amount}")]
    public static partial void InsufficientBalance(this ILogger logger, Guid walletId, decimal amount);

    [LoggerMessage(LogEvents.ConcurrencyConflict, LogLevel.Warning,
        "Xung đột concurrency khi cập nhật ví {WalletId}, sẽ thử lại lần {AttemptNumber}")]
    public static partial void ConcurrencyConflict(this ILogger logger, Guid walletId, int attemptNumber);

    [LoggerMessage(LogEvents.TransactionCreated, LogLevel.Information,
        "Tạo giao dịch {TransactionId} loại {TransactionType}, số tiền {Amount}")]
    public static partial void TransactionCreated(this ILogger logger, Guid transactionId, string transactionType, decimal amount);

    [LoggerMessage(LogEvents.TransactionSucceeded, LogLevel.Information,
        "Giao dịch {TransactionId} thành công")]
    public static partial void TransactionSucceeded(this ILogger logger, Guid transactionId);

    [LoggerMessage(LogEvents.TransactionFailed, LogLevel.Error,
        "Giao dịch {TransactionId} thất bại: {Reason}")]
    public static partial void TransactionFailed(this ILogger logger, Guid transactionId, string reason);

    [LoggerMessage(LogEvents.IdempotentRequestSkipped, LogLevel.Information,
        "Request với Idempotency-Key {IdempotencyKey} đã được xử lý trước đó, trả kết quả cache")]
    public static partial void IdempotentRequestSkipped(this ILogger logger, string idempotencyKey);
}