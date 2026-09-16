namespace WalletFlow.Application.Common.Constants;

public static class ErrorCodes
{
    public static class Auth
    {
        public const string UsernameExists = "USERNAME_EXISTS";
        public const string PhoneExists = "PHONE_EXISTS";
        public const string EmailExists = "EMAIL_EXISTS";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string AccountLocked = "ACCOUNT_LOCKED";
        public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string InvalidOtp = "INVALID_OTP";
        public const string ValidationError = "VALIDATION_ERROR";
    }

    public static class Wallet
    {
        public const string NotFound = "WALLET_NOT_FOUND";
        public const string AlreadyExists = "WALLET_ALREADY_EXISTS";
        public const string ConcurrencyConflict = "WALLET_CONCURRENCY_CONFLICT";
        public const string InvalidStatusChange = "INVALID_WALLET_STATUS_CHANGE";
    }

    public static class Transaction
    {
        public const string InsufficientBalance = "INSUFFICIENT_BALANCE";
        public const string SourceWalletNotFound = "SOURCE_WALLET_NOT_FOUND";
        public const string DestinationWalletNotFound = "DESTINATION_WALLET_NOT_FOUND";
        public const string CurrencyMismatch = "CURRENCY_MISMATCH";
    }

    public static class Common
    {
        public const string ValidationError = "VALIDATION_ERROR";
        public const string DomainError = "DOMAIN_ERROR";
        public const string InternalError = "INTERNAL_ERROR";
        public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";
    }
}