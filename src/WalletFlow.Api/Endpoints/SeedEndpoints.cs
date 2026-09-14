using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Infrastructure.Persistence;

namespace WalletFlow.Api.Endpoints;

public static class SeedEndpoints
{
    public static void MapSeedEndpoints(this IEndpointRouteBuilder app)
    {
        if (!app.ServiceProvider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            return;

        app.MapPost("/api/dev/seed", async (AppDbContext dbContext, IPasswordHasher passwordHasher, IWalletSettingsProvider walletSettings, ILogger<Program> logger) =>
        {
            if (dbContext.Users.Any())
                return Results.Ok(new { message = "Database đã có dữ liệu, bỏ qua seed." });

            await DataSeeder.SeedAsync(dbContext, passwordHasher, walletSettings, logger);

            return Results.Ok(new
            {
                message = "Seed thành công.",
                accounts = new[]
                {
                    new { role = "Admin", username = "admin", password = "Admin@123" },
                    new { role = "User", username = "testuser", password = "User@123" }
                }
            });
        })
        .WithName("SeedDevData")
        .WithTags("Dev")
        .WithSummary("[DEV ONLY] Tạo tài khoản Admin + User mẫu để test");
    }
}