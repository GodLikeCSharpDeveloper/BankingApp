using System.Threading.Channels;

namespace BankingApp.Services.Queue
{
    public sealed class ChannelOperationQueue : IOperationQueue, IAsyncDisposable
    {
        private readonly ILogger<ChannelOperationQueue> _logger;
        private readonly Channel<Func<CancellationToken, Task>> _channel;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _consumer;

        public ChannelOperationQueue(ILogger<ChannelOperationQueue> logger)
        {
            _channel = Channel.CreateUnbounded<Func<CancellationToken, Task>>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
            _consumer = Task.Run(ConsumeAsync);
            _logger = logger;
        }

        public Task<T> EnqueueAsync<T>(Func<Task<T>> op, CancellationToken ct = default)
        {
            return EnqueueInternalAsync(op, ct);
        }

        public Task EnqueueAsync(Func<CancellationToken, Task> op, CancellationToken ct = default)
        {
            return EnqueueInternalAsync(async () =>
            {
                await op(ct);
                return true;
            }, ct);
        }

        private Task<T> EnqueueInternalAsync<T>(Func<Task<T>> op, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            var written = _channel.Writer.TryWrite(async token =>
            {
                try { tcs.SetResult(await op().WaitAsync(token)); }
                catch (Exception ex) { tcs.SetException(ex); }
            });

            if (!written)
                throw new InvalidOperationException("Operation queue is closed.");

            return tcs.Task.WaitAsync(ct);
        }

        private async Task ConsumeAsync()
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(_cts.Token))
                {
                    while (_channel.Reader.TryRead(out var work))
                    {
                        try
                        {
                            await work(_cts.Token);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error occurred executing operation from queue");
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (_cts.IsCancellationRequested) { }
        }

        public async ValueTask DisposeAsync()
        {
            _channel.Writer.TryComplete();
            await _cts.CancelAsync();
            try
            {
                await _consumer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred stopping operation queue consumer");
            }
            _cts.Dispose();
        }
    }
}