using WalletFlow.Application.Common.Interfaces;

namespace WalletFlow.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}