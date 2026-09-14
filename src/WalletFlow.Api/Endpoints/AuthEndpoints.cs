using MediatR;
using WalletFlow.Application.Auth.Commands.ForgotPassword;
using WalletFlow.Application.Auth.Commands.Login;
using WalletFlow.Application.Auth.Commands.Token;
using WalletFlow.Application.Auth.Commands.Register;
using WalletFlow.Application.Auth.Commands.ResetPassword;
using WalletFlow.Application.Auth.Common;
using WalletFlow.Application.Common.Models;

namespace WalletFlow.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok(new ApiResponse<AuthResponse>(result.Value!, "Đăng ký tài khoản thành công."))
                : Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("Register")
        .WithSummary("Đăng ký tài khoản mới")
        .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/login", async (LoginCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok(new ApiResponse<AuthResponse>(result.Value!, "Đăng nhập thành công."))
                : Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("Login")
        .WithSummary("Đăng nhập bằng username hoặc số điện thoại")
        .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/refresh-token", async (RefreshTokenCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok(new ApiResponse<AuthResponse>(result.Value!, "Làm mới token thành công."))
                : Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("RefreshToken")
        .WithSummary("Làm mới access token bằng refresh token")
        .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/forgot-password", async (ForgotPasswordCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok(new ApiResponse("Mã OTP đã được gửi đi, mã sẽ hết hạn sau 10 phút."))
                : Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("ForgotPassword")
        .WithSummary("Gửi mã OTP về số điện thoại để đặt lại mật khẩu")
        .Produces<ApiResponse>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status429TooManyRequests)
        .RequireRateLimiting("otp");

        group.MapPost("/reset-password", async (ResetPasswordCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Ok(new ApiResponse("Đặt lại mật khẩu thành công."))
                : Results.BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));
        })
        .WithName("ResetPassword")
        .WithSummary("Xác nhận OTP và đặt lại mật khẩu mới")
        .Produces<ApiResponse>(StatusCodes.Status200OK)
        .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);
    }
}