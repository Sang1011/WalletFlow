namespace WalletFlow.Application.Common.Models;

public record ApiResponse<T>(T Data, string Message);

public record ApiResponse(string Message);

public record ApiErrorResponse(string Error, string? ErrorCode, IReadOnlyList<string>? Details = null);