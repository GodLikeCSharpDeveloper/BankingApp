using BankingApp.Entities;
using BankingApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankingAppTest.Repositories
{
    public class EfRepositoryTests
    {
        private readonly EfRepository<AccountEntity> efRepository;

        public EfRepositoryTests()
        {
            efRepository = new(DbHelper.GetContext());
        }

        [Fact]
        public async Task CreateAsync_AddsAccount()
        {
            // Arrange
            var account = new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 1000
            };

            await efRepository.CreateAsync(account);
            await efRepository.SaveChangesAsync();

            // Act
            var found = await efRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(1000, found.Balance);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange & Act
            var account = await efRepository.GetByIdAsync(999);

            // Assert
            Assert.Null(account);
        }

        [Fact]
        public async Task DeleteAsync_RemovesAccount()
        {
            // Arrange
            var account = new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 1000
            };
            await efRepository.CreateAsync(account);
            await efRepository.SaveChangesAsync();

            // Act
            await efRepository.DeleteAsync(1);
            await efRepository.SaveChangesAsync();
            var found = await efRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(found);
        }

        [Fact]
        public async Task AsQueryable_ReturnsAllAccounts()
        {
            // Arrange
            var account1 = new AccountEntity { AccountNumber = "ACC123", Balance = 1000 };
            var account2 = new AccountEntity { AccountNumber = "ACC456", Balance = 2000 };
            await efRepository.CreateAsync(account1);
            await efRepository.CreateAsync(account2);
            await efRepository.SaveChangesAsync();

            // Act
            var accounts = await efRepository.AsQueryable().ToListAsync();

            // Assert
            Assert.Equal(2, accounts.Count);
        }
    }
}