namespace WalletFlow.Application.Common.Interfaces;

public interface IIdempotentRequest
{
    string IdempotencyKey { get; }
}