using MediatR;
using WalletFlow.Application.Auth.Commands.ForgotPassword;
using WalletFlow.Application.Auth.Commands.Login;
using WalletFlow.Application.Auth.Commands.Token;
using WalletFlow.Application.Auth.Commands.Register;
using WalletFlow.Application.Auth.Commands.ResetPassword;

namespace WalletFlow.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { result.Error, result.ErrorCode });
        });

        group.MapPost("/login", async (LoginCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { result.Error, result.ErrorCode });
        });

        group.MapPost("/refresh-token", async (RefreshTokenCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { result.Error, result.ErrorCode });
        });

        group.MapPost("/forgot-password", async (ForgotPasswordCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error, result.ErrorCode });
        });

        group.MapPost("/reset-password", async (ResetPasswordCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error, result.ErrorCode });
        });
    }
}