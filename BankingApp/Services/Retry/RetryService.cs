using Polly;
using Polly.Retry;

namespace BankingApp.Services.Retry
{
    public class RetryService : IRetryService
    {
        public async Task ExecuteAsync(
            Func<Task> action,
            Func<Exception, int, Task> onRetry,
            int retryCount = 3,
            TimeSpan onRetryDelay = default,
            Action? onFailure = null)
        {
            if (onRetryDelay == default) onRetryDelay = TimeSpan.FromSeconds(1);

            AsyncRetryPolicy policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount,
                    _ => onRetryDelay,
                    async (ex, _, attempt, _) => await onRetry(ex, attempt).ConfigureAwait(false)
                );

            try
            {
                await policy.ExecuteAsync(action).ConfigureAwait(false);
            }
            catch
            {
                onFailure?.Invoke();
                throw;
            }
        }
    }
}