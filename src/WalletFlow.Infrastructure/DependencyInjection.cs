using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Infrastructure.Identity;
using WalletFlow.Infrastructure.Persistence;
using WalletFlow.Infrastructure.Services;
using WalletFlow.Infrastructure.Settings;

namespace WalletFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddHttpContextAccessor();
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost:6379"));
        services.Configure<OtpSettings>(configuration.GetSection("Otp"));
        services.AddScoped<IOtpSettingsProvider, OtpSettingsProvider>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IIdempotencyService, RedisIdempotencyService>();
        services.AddScoped<ISmsSender, FakeSmsSender>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}