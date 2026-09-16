using Microsoft.Extensions.Logging;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Infrastructure.Persistence;

namespace WalletFlow.Infrastructure.BackgroundJobs;

public class OutboxProcessorJob
{
    private readonly AppDbContext _dbContext;
    private readonly ISmsSender _smsSender;
    private readonly ILogger<OutboxProcessorJob> _logger;

    public OutboxProcessorJob(AppDbContext dbContext, ISmsSender smsSender, ILogger<OutboxProcessorJob> logger)
    {
        _dbContext = dbContext;
        _smsSender = smsSender;
        _logger = logger;
    }

    public async Task ProcessPendingMessagesAsync()
    {
        var pendingMessages = _dbContext.OutboxMessages
            .Where(m => m.ProcessedAtUtc == null && m.RetryCount < 5)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(50)
            .ToList();

        if (pendingMessages.Count == 0)
            return;

        _logger.LogInformation("Outbox: xử lý {Count} message đang chờ.", pendingMessages.Count);

        foreach (var message in pendingMessages)
        {
            try
            {
                _logger.LogInformation(
                    "[OUTBOX] Gửi thông báo loại {Type}: {Payload}", message.Type, message.Payload);

                message.MarkProcessed();
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
                _logger.LogError(ex, "Outbox: xử lý message {MessageId} thất bại.", message.Id);
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}