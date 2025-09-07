namespace BankingApp.Services.Queue
{
    public interface IOperationQueue
    {
        Task<T> EnqueueAsync<T>(Func<Task<T>> op, CancellationToken ct = default);
        Task EnqueueAsync(Func<CancellationToken, Task> op, CancellationToken ct = default);
    }
}