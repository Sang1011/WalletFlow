using System.Net;
using System.Text.Json;
using FluentValidation;
using WalletFlow.Application.Common.Constants;
using WalletFlow.Application.Common.Models;
using WalletFlow.Domain.Exceptions;

namespace WalletFlow.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest,
                new ApiErrorResponse(
                    "Dữ liệu không hợp lệ.",
                    ErrorCodes.Common.ValidationError,
                    ex.Errors.Select(e => e.ErrorMessage).ToList()));
        }
        catch (DomainException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest,
                new ApiErrorResponse(ex.Message, ErrorCodes.Common.DomainError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi không xác định");
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError,
                new ApiErrorResponse("Đã có lỗi xảy ra, vui lòng thử lại sau.", ErrorCodes.Common.InternalError));
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, ApiErrorResponse error)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    }
}