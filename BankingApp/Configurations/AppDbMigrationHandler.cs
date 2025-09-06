using BankingApp.Services.Retry;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Configurations
{
    public class AppDbMigrationHandler(IServiceScopeFactory serviceScopeFactory, ILogger<AppDbMigrationHandler> logger, IRetryService retryService) : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly IRetryService _retryService = retryService;
        private readonly ILogger<AppDbMigrationHandler> _logger = logger;
        private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(5);
        private const int _maxRetryCount = 5;
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await _retryService.ExecuteAsync(
                action: () => db.Database.MigrateAsync(),
                onRetry: (ex, attempt) =>
                {
                    _logger.LogError(ex, "An error occurred while migrating the database. Attempt {Attempt}", attempt);
                    return Task.CompletedTask;
                },
                onRetryDelay: _retryDelay,
                retryCount: _maxRetryCount,
                onFailure: () => _logger.LogCritical("Database migration failed after {RetryCount} retries", _maxRetryCount)
            );
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}