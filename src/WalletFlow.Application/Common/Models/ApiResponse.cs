namespace WalletFlow.Application.Common.Models;

public record ApiResponse<T>(T Data);

public record ApiErrorResponse(string Error, string? ErrorCode, IReadOnlyList<string>? Details = null);