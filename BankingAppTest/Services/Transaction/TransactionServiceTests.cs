using BankingApp.Configurations;
using BankingApp.Entities;
using BankingApp.Services.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BankingAppTest.Services.Transaction
{
    public class TransactionServiceTests
    {
        private readonly AppDbContext _dbContext;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _dbContext = DbHelper.GetContext();
            _transactionService = new TransactionService(_dbContext);
        }

        [Fact]
        public async Task ExecuteAsync_RollsBackTransaction_WhenExceptionThrown()
        {
            // Arrange
            var initialCount = await _dbContext.Accounts.CountAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _transactionService.ExecuteAsync(async (context) =>
                {
                    await AddAccountAsync(context);
                    throw new InvalidOperationException("Simulated exception");
                });
            });
            var finalCount = await _dbContext.Accounts.CountAsync();
            Assert.Equal(initialCount, finalCount);
        }

        private Task<EntityEntry<AccountEntity>> AddAccountAsync(AppDbContext appDbContext)
        {
            return appDbContext.Accounts.AddAsync(new AccountEntity { AccountNumber = "ACC000001" }).AsTask();
        }
    }
}