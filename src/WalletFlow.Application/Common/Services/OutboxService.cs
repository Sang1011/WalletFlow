using System.Text.Json;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Domain.Entities;

namespace WalletFlow.Application.Common.Services;

public class OutboxService : IOutboxService
{
    private readonly IAppDbContext _dbContext;

    public OutboxService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Enqueue<T>(string type, T payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var message = OutboxMessage.Create(type, json);
        _dbContext.OutboxMessages.Add(message);
    }
}