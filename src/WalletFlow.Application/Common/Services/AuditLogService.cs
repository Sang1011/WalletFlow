using System.Text.Json;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Application.Common.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public AuditLogService(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public void Log(string action, string entityName, Guid entityId, object? oldValue, object? newValue)
    {
        var log = AuditLog.Create(
            _currentUserService.UserId,
            action,
            entityName,
            entityId,
            oldValue is null ? null : JsonSerializer.Serialize(oldValue),
            newValue is null ? null : JsonSerializer.Serialize(newValue));

        _dbContext.AuditLogs.Add(log);
    }
}