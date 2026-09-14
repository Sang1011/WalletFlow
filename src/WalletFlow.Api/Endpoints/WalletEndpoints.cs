using MediatR;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Wallets.Commands.CreateWallet;
using WalletFlow.Application.Wallets.Common;
using WalletFlow.Application.Wallets.Queries.GetMyWallets;
using WalletFlow.Application.Wallets.Queries.GetWalletById;

namespace WalletFlow.Api.Endpoints;

public static class WalletEndpoints
{
    public static void MapWalletEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wallets")
            .WithTags("Wallets")
            .RequireAuthorization();

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetMyWalletsQuery());
            return result.IsSuccess
                ? Results.Ok(new ApiResponse<List<WalletResponse>>(result.Value!, "Lấy danh sách ví thành công."))
                : Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("GetMyWallets")
        .WithSummary("Lấy danh sách ví của tôi")
        .Produces<ApiResponse<List<WalletResponse>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/{walletId:guid}", async (Guid walletId, ISender sender) =>
        {
            var result = await sender.Send(new GetWalletByIdQuery(walletId));
            return result.IsSuccess
                ? Results.Ok(new ApiResponse<WalletResponse>(result.Value!, "Lấy thông tin ví thành công."))
                : Results.NotFound(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("GetWalletById")
        .WithSummary("Xem chi tiết 1 ví theo Id")
        .Produces<ApiResponse<WalletResponse>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/", async (CreateWalletCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok(new ApiResponse<WalletResponse>(result.Value!, "Tạo ví mới thành công."))
                : Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("CreateWallet")
        .WithSummary("Tạo thêm ví mới (loại tiền tệ khác)")
        .Produces<ApiResponse<WalletResponse>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}