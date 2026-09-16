namespace WalletFlow.Application.Common.Interfaces;

public interface IAuditLogService
{
    void Log(string action, string entityName, Guid entityId, object? oldValue, object? newValue);
}