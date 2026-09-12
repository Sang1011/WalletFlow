namespace WalletFlow.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<bool> HasBeenProcessedAsync(string idempotencyKey, CancellationToken cancellationToken = default);
    Task<T?> GetCachedResultAsync<T>(string idempotencyKey, CancellationToken cancellationToken = default);
    Task SaveResultAsync<T>(string idempotencyKey, T result, CancellationToken cancellationToken = default);
}