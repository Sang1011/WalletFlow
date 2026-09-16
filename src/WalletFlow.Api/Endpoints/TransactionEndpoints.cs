using MediatR;
using WalletFlow.Application.Common.Constants;
using WalletFlow.Application.Common.Models;
using WalletFlow.Application.Transactions.Commands.Deposit;
using WalletFlow.Application.Transactions.Commands.Transfer;
using WalletFlow.Application.Transactions.Commands.Withdraw;
using WalletFlow.Application.Transactions.Common;
using WalletFlow.Application.Transactions.Queries.GetTransactionHistory;

namespace WalletFlow.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions")
            .WithTags("Transactions")
            .RequireAuthorization();

        group.MapPost("/deposit", async (DepositCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return MapResult(result);
        })
        .WithName("Deposit")
        .WithSummary("Nạp tiền vào ví");

        group.MapPost("/withdraw", async (WithdrawCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return MapResult(result);
        })
        .WithName("Withdraw")
        .WithSummary("Rút tiền khỏi ví");

        group.MapPost("/transfer", async (TransferCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return MapResult(result);
        })
        .WithName("Transfer")
        .WithSummary("Chuyển tiền giữa 2 ví");

        group.MapGet("/wallet/{walletId:guid}", async (
            Guid walletId, int pageNumber, int pageSize, string? type, string? status, ISender sender) =>
        {
            var result = await sender.Send(new GetTransactionHistoryQuery(
                walletId,
                pageNumber == 0 ? 1 : pageNumber,
                pageSize == 0 ? 20 : pageSize,
                type, status));

            return result.IsSuccess
                ? Results.Ok(new ApiResponse<PaginatedList<TransactionResponse>>(result.Value!, "Lấy lịch sử giao dịch thành công."))
                : Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("GetTransactionHistory")
        .WithSummary("Xem lịch sử giao dịch của 1 ví (phân trang, filter)");
    }

    private static IResult MapResult(Result<TransactionResponse> result)
    {
        if (result.IsSuccess)
            return Results.Ok(new ApiResponse<TransactionResponse>(result.Value!, "Giao dịch thành công."));

        return result.ErrorCode switch
        {
            ErrorCodes.Wallet.ConcurrencyConflict => Results.Conflict(new ApiErrorResponse(result.Error!, result.ErrorCode)),
            ErrorCodes.Wallet.NotFound or ErrorCodes.Transaction.SourceWalletNotFound or ErrorCodes.Transaction.DestinationWalletNotFound
                => Results.NotFound(new ApiErrorResponse(result.Error!, result.ErrorCode)),
            _ => Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode))
        };
    }
}