namespace WalletFlow.Application.Common.Interfaces;

public interface IOutboxService
{
    void Enqueue<T>(string type, T payload);
}