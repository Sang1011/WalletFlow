using WalletFlow.Domain.Common;

namespace WalletFlow.Domain.Entities;

public class OutboxMessage : BaseEntity
{
    public string Type { get; private set; } = default!;
    public string Payload { get; private set; } = default!; // JSON
    public DateTime? ProcessedAtUtc { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }

    private OutboxMessage() { }

    public static OutboxMessage Create(string type, string payloadJson)
    {
        return new OutboxMessage
        {
            Type = type,
            Payload = payloadJson,
            RetryCount = 0
        };
    }

    public bool IsProcessed => ProcessedAtUtc.HasValue;

    public void MarkProcessed() => ProcessedAtUtc = DateTime.UtcNow;

    public void MarkFailed(string error)
    {
        RetryCount++;
        Error = error;
    }
}