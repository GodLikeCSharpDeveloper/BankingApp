using BankingApp.Services.Retry;

namespace BankingAppTest.Services.Retry
{
    public class RetryServiceTests
    {
        private readonly RetryService retryService;

        public RetryServiceTests()
        {
            retryService = new RetryService();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRetry_OnException()
        {
            // Arrange
            int attempt = 0;
            int maxAttempts = 3;
            Task action()
            {
                attempt++;
                if (attempt < maxAttempts)
                {
                    throw new Exception("Transient error");
                }
                return Task.CompletedTask;
            }
            int retryCount = 0;
            Task onRetry(Exception ex, int att)
            {
                retryCount++;
                return Task.CompletedTask;
            }

            // Act
            await retryService.ExecuteAsync(action, onRetry, onRetryDelay: TimeSpan.FromMilliseconds(1), retryCount: maxAttempts);

            // Assert
            Assert.Equal(maxAttempts - 1, retryCount);
            Assert.Equal(maxAttempts, attempt);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallOnFailure_AfterMaxRetries()
        {
            // Arrange
            int attempt = 0;
            int maxAttempts = 3;
            Task action()
            {
                attempt++;
                throw new Exception("Transient error");
            }
            int retryCount = 0;
            Task onRetry(Exception ex, int att)
            {
                retryCount++;
                return Task.CompletedTask;
            }
            bool onFailureCalled = false;
            void onFailure()
            {
                onFailureCalled = true;
            }
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await retryService.ExecuteAsync(action, onRetry, retryCount: maxAttempts, onRetryDelay: TimeSpan.FromMilliseconds(1), onFailure: onFailure)
            );
            Assert.Equal(maxAttempts, retryCount);
            Assert.Equal(maxAttempts + 1, attempt);
            Assert.True(onFailureCalled);
        }
    }
}