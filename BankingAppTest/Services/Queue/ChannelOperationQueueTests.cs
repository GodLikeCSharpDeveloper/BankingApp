using BankingApp.Services.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BankingAppTest.Services.Queue
{
    public class ChannelOperationQueueTests
    {
        private readonly ILogger<ChannelOperationQueue> _logger = NullLogger<ChannelOperationQueue>.Instance;
        private readonly ChannelOperationQueue _queue;

        public ChannelOperationQueueTests()
        {
            _queue = new ChannelOperationQueue(_logger);
        }
        [Fact]
        public async Task Operation_Is_Executed()
        {
            var flag = false;
            await _queue.EnqueueAsync(ct =>
            {
                flag = true;
                return Task.CompletedTask;
            });

            Assert.True(flag);
            await _queue.DisposeAsync();
        }

        [Fact]
        public async Task Operations_Are_Executed_In_Order()
        {
            var result = new List<int>();

            await Task.WhenAll(
                _queue.EnqueueAsync(async _ => { await Task.Delay(10); result.Add(1); }),
                _queue.EnqueueAsync(async _ => { await Task.Delay(5); result.Add(2); }),
                _queue.EnqueueAsync(async _ => { result.Add(3); }));

            Assert.Equal([1, 2, 3], result);
            await _queue.DisposeAsync();
        }

        [Fact]
        public async Task Operation_Exception_Is_Propagated()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _queue.EnqueueAsync(_ => throw new InvalidOperationException()));

            Assert.NotNull(ex);
            await _queue.DisposeAsync();
        }

        [Fact]
        public async Task Cancelled_Token_Throws()
        {
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _queue.EnqueueAsync(_ => Task.CompletedTask, cts.Token));

            await _queue.DisposeAsync();
        }

        [Fact]
        public async Task Cannot_Enqueue_After_Dispose()
        {
            await _queue.DisposeAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _queue.EnqueueAsync(_ => Task.CompletedTask));
        }
    }
}