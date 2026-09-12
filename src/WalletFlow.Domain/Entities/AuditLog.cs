using WalletFlow.Domain.Common;

namespace WalletFlow.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = default!;
    public string EntityName { get; private set; } = default!;
    public Guid EntityId { get; private set; }
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        Guid? userId,
        string action,
        string entityName,
        Guid entityId,
        string? oldValue,
        string? newValue)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValue = oldValue,
            NewValue = newValue
        };
    }
}