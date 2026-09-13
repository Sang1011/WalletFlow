namespace WalletFlow.Application.Auth.Common;

public record AuthResponse(
    Guid UserId,
    string Username,
    string PhoneNumber,
    string Email,
    string FullName,
    string AccessToken,
    string RefreshToken);