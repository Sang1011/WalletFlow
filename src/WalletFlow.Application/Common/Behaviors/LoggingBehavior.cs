using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WalletFlow.Application.Common.Logging;

namespace WalletFlow.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid();
        var stopwatch = Stopwatch.StartNew();

        _logger.RequestStarted(requestName, requestGuid);

        try
        {
            var response = await next();
            stopwatch.Stop();
            _logger.RequestCompleted(requestName, requestGuid, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            _logger.RequestFailed(ex, requestName, requestGuid);
            throw;
        }
    }
}