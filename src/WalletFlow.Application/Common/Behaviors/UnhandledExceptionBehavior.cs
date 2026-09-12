using MediatR;
using Microsoft.Extensions.Logging;

namespace WalletFlow.Application.Common.Behaviors;

public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex) when (ex is not FluentValidation.ValidationException)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, "Lỗi không xác định khi xử lý {RequestName}", requestName);
            throw;
        }
    }
}