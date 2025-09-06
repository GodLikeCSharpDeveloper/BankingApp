using System.Collections.Concurrent;

namespace BankingApp.Services.Funds
{
    public class InMemoryOperationQueue : IOperationQueue
    {
        private readonly ConcurrentQueue<Func<Task>> _queue = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public Task EnqueueAsync(Func<Task> operation)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            _queue.Enqueue(async () =>
            {
                try { await operation(); tcs.SetResult(); }
                catch (Exception ex) { tcs.SetException(ex); }
            });

            _ = ProcessQueueAsync();
            return tcs.Task;
        }

        private async Task ProcessQueueAsync()
        {
            if (!await _semaphore.WaitAsync(0)) return;

            try
            {
                while (_queue.TryDequeue(out var next))
                {
                    await next();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}