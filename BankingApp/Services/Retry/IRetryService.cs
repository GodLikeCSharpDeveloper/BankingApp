namespace BankingApp.Services.Retry
{
    public interface IRetryService
    {
        Task ExecuteAsync(
            Func<Task> action,
            Func<Exception, int, Task> onRetry,
            int retryCount = 3,
            TimeSpan onRetryDelay = default,
            Action? onFailure = null);
    }
}