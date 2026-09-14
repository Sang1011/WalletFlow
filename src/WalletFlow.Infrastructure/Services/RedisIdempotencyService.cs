using System.Text.Json;
using StackExchange.Redis;
using WalletFlow.Application.Common.Interfaces;

namespace WalletFlow.Infrastructure.Services;

public class RedisIdempotencyService : IIdempotencyService
{
    private readonly IConnectionMultiplexer _redis;

    private static readonly TimeSpan DefaultTtl = TimeSpan.FromHours(24);

    public RedisIdempotencyService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<bool> HasBeenProcessedAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync(BuildKey(idempotencyKey));
    }

    public async Task<T?> GetCachedResultAsync<T>(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var value = await db.StringGetAsync(BuildKey(idempotencyKey));

        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task SaveResultAsync<T>(string idempotencyKey, T result, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(result);
        await db.StringSetAsync(BuildKey(idempotencyKey), json, DefaultTtl);
    }

    private static string BuildKey(string idempotencyKey) => $"idempotency:{idempotencyKey}";
}