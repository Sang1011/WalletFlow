using System.Net;
using System.Text.Json;
using FluentValidation;
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
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, "VALIDATION_ERROR",
                ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (DomainException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, "DOMAIN_ERROR", new[] { ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi không xác định");
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "INTERNAL_ERROR",
                new[] { "Đã có lỗi xảy ra, vui lòng thử lại sau." });
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context, HttpStatusCode statusCode, string code, IEnumerable<string> errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new { code, errors });
        await context.Response.WriteAsync(payload);
    }
}