using Microsoft.EntityFrameworkCore;
using WalletFlow.Application.Common.Interfaces;
using WalletFlow.Infrastructure.Persistence;

namespace WalletFlow.Api.Endpoints;

public static class SeedEndpoints
{
    public static void MapSeedEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dev/seed")
            .WithTags("Development");

        group.MapPost("/", async (
            AppDbContext dbContext,
            IPasswordHasher passwordHasher,
            ILogger<Program> logger) =>
        {
            if (await dbContext.Users.AnyAsync())
            {
                return Results.Conflict(new
                {
                    code = "DATABASE_ALREADY_SEEDED",
                    message = "Database đã có dữ liệu, không cần seed."
                });
            }

            await DataSeeder.SeedAsync(
                dbContext,
                passwordHasher,
                logger);

            return Results.Ok(new
            {
                code = "SEED_SUCCESS",
                message = "Seed dữ liệu thành công."
            });
        })
        .WithName("SeedDatabase")
        .WithSummary("Seed dữ liệu mặc định cho database");
    }
}