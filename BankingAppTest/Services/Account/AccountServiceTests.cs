using AutoMapper;
using BankingApp.Configurations;
using BankingApp.Entities;
using BankingApp.Models;
using BankingApp.Repositories.Account;
using BankingApp.Services.Account;
using EntityFrameworkCore.Testing.Moq;
using Moq;

namespace BankingAppTest.Services.Account
{
    public class AccountServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IAccountNumberGenerator> _accountNumberGeneratorMock;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _accountNumberGeneratorMock = new Mock<IAccountNumberGenerator>();
            _accountService = new AccountService(_accountRepositoryMock.Object, _accountNumberGeneratorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateAccountAndReturnGetAccountDto()
        {
            // Arrange
            var createAccountDto = new CreateAccountDto { InitialBalance = 1000m };
            var accountEntity = new AccountEntity { Id = 1, Balance = 1000m, AccountNumber = "1234567890" };
            var getAccountDto = new GetAccountDto { Balance = 1000m, AccountNumber = "1234567890" };
            _mapperMock.Setup(m => m.Map<AccountEntity>(createAccountDto)).Returns(accountEntity);
            _accountNumberGeneratorMock.Setup(ang => ang.GenerateAsync()).ReturnsAsync("1234567890");
            _mapperMock.Setup(m => m.Map<GetAccountDto>(accountEntity)).Returns(getAccountDto);

            // Act
            var result = await _accountService.CreateAsync(createAccountDto);

            // Assert
            _accountRepositoryMock.Verify(ar => ar.CreateAsync(accountEntity), Times.Once);
            _accountRepositoryMock.Verify(ar => ar.SaveChangesAsync(), Times.Once);
            Assert.Equal(getAccountDto, result);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfGetAccountDto()
        {
            // Arrange
            var accountEntities = new List<AccountEntity>
            {
                new() { Id = 1, Balance = 1000m, AccountNumber = "1234567890" },
                new() { Id = 2, Balance = 2000m, AccountNumber = "0987654321" }
            };

            var getAccountDtos = new List<GetAccountDto>
            {
                new() { Balance = 1000m, AccountNumber = "1234567890" },
                new() { Balance = 2000m, AccountNumber = "0987654321" }
            };

            var mockDbContext = Create.MockedDbContextFor<AppDbContext>();
            mockDbContext.Set<AccountEntity>().AddRange(accountEntities);

            var repository = new AccountRepository(mockDbContext);

            _mapperMock.Setup(m => m.Map<List<GetAccountDto>>(It.IsAny<List<AccountEntity>>()))
                       .Returns(getAccountDtos);

            var service = new AccountService(repository, _accountNumberGeneratorMock.Object, _mapperMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(getAccountDtos, result);
        }

        [Fact]
        public async Task FindByAccountNumberAsync_ShouldReturnGetAccountDto_WhenAccountExists()
        {
            // Arrange
            var accountNumber = "1234567890";
            var accountEntity = new AccountEntity { Id = 1, Balance = 1000m, AccountNumber = accountNumber };
            var getAccountDto = new GetAccountDto { Balance = 1000m, AccountNumber = accountNumber };
            _accountRepositoryMock.Setup(ar => ar.FindByAccountNumberAsync(accountNumber)).ReturnsAsync(accountEntity);
            _mapperMock.Setup(m => m.Map<GetAccountDto>(accountEntity)).Returns(getAccountDto);

            // Act
            var result = await _accountService.FindByAccountNumberAsync(accountNumber);

            // Assert
            Assert.Equal(getAccountDto, result);
        }

        [Fact]
        public async Task FindByAccountNumberAsync_ShouldReturnNull_WhenAccountDoesNotExist()
        {
            // Arrange
            var accountNumber = "1234567890";
            _accountRepositoryMock.Setup(ar => ar.FindByAccountNumberAsync(accountNumber)).ReturnsAsync((AccountEntity?)null);

            // Act
            var result = await _accountService.FindByAccountNumberAsync(accountNumber);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateByAccountNumberAsync_ShouldUpdateAccountBalance_WhenAccountExists()
        {
            // Arrange
            var updateAccountDto = new UpdateAccountDto { AccountNumber = "1234567890", Balance = 1500m };
            var accountEntity = new AccountEntity { Id = 1, Balance = 1000m, AccountNumber = "1234567890" };
            _accountRepositoryMock.Setup(ar => ar.FindTrackedByAccountNumberAsync(updateAccountDto.AccountNumber)).ReturnsAsync(accountEntity);

            // Act
            await _accountService.UpdateByAccountNumberAsync(updateAccountDto);

            // Assert
            Assert.Equal(1500m, accountEntity.Balance);
            _accountRepositoryMock.Verify(ar => ar.SaveChangesAsync(), Times.Once);
        }
    }
}