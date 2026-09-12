using MediatR;
using Microsoft.Extensions.Logging;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Application.Common.Logging;

namespace WalletFlow.Application.Common.Behaviors;

public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(
        IIdempotencyService idempotencyService,
        ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _idempotencyService = idempotencyService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IIdempotentRequest idempotentRequest)
            return await next();

        var key = idempotentRequest.IdempotencyKey;

        var cached = await _idempotencyService.GetCachedResultAsync<TResponse>(key, cancellationToken);
        if (cached is not null)
        {
            _logger.IdempotentRequestSkipped(key);
            return cached;
        }

        var response = await next();
        await _idempotencyService.SaveResultAsync(key, response, cancellationToken);
        return response;
    }
}