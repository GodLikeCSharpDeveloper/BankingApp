using BankingApp.Entities;
using BankingApp.Repositories.Account;

namespace BankingAppTest.Repositories.Account
{
    public class AccountRepositoryTests
    {
        private readonly AccountRepository accountRepository;

        public AccountRepositoryTests()
        {
            accountRepository = new(DbHelper.GetContext());
        }

        [Fact]
        public async Task FindByAccountNumberAsync_ReturnsAccount_WhenExists()
        {
            // Arrange
            var account = new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 1000
            };
            await accountRepository.CreateAsync(account);
            await accountRepository.SaveChangesAsync();

            // Act
            var found = await accountRepository.FindByAccountNumberAsync("ACC123");

            // Assert
            Assert.NotNull(found);
            Assert.Equal(1000, found.Balance);
        }

        [Fact]
        public async Task FindByAccountNumberAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange & Act
            var account = await accountRepository.FindByAccountNumberAsync("NONEXISTENT");

            // Assert
            Assert.Null(account);
        }
    }
}