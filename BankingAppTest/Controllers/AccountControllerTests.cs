using BankingApp.Controllers;
using BankingApp.Models;
using BankingApp.Services.Account;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankingAppTest.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly AccountController _accountController;
        public AccountControllerTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _accountController = new AccountController(_accountServiceMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfAccounts()
        {
            // Arrange
            var accounts = new List<GetAccountDto> { new() { AccountNumber = "123", Balance = 1000 } };
            _accountServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(accounts);

            // Act
            var result = await _accountController.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(accounts, okResult.Value);
        }

        [Fact]
        public async Task GetByAccountNumberAsync_ExistingAccount_ReturnsOkResult_WithAccount()
        {
            // Arrange
            var account = new GetAccountDto { AccountNumber = "123", Balance = 1000 };
            _accountServiceMock.Setup(s => s.FindByAccountNumberAsync("123")).ReturnsAsync(account);

            // Act
            var result = await _accountController.GetByAccountNumberAsync("123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(account, okResult.Value);
        }

        [Fact]
        public async Task GetByAccountNumberAsync_NonExistingAccount_ReturnsNotFoundResult()
        {
            // Arrange
            _accountServiceMock.Setup(s => s.FindByAccountNumberAsync("999")).ReturnsAsync((GetAccountDto?)null);

            // Act
            var result = await _accountController.GetByAccountNumberAsync("999");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateAsync_ValidAccount_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var createAccountDto = new CreateAccountDto { InitialBalance = 500 };
            var createdAccount = new GetAccountDto { AccountNumber = "123", Balance = 500 };
            _accountServiceMock.Setup(s => s.CreateAsync(createAccountDto)).ReturnsAsync(createdAccount);

            // Act
            var result = await _accountController.CreateAsync(createAccountDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_accountController.GetByAccountNumberAsync), createdAtActionResult.ActionName);
            Assert.Equal(createdAccount, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateAsync_NotValidAccount_ReturnsBadRequest()
        {
            // Arrange
            var createAccountDto = new CreateAccountDto { InitialBalance = -1 };
            _accountController.ModelState.AddModelError("InitialBalance", "The field InitialBalance must be between 0 and 79228162514264337593543950335.");

            // Act
            var result = await _accountController.CreateAsync(createAccountDto);

            // Assert
            _accountServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateAccountDto>()), Times.Never);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}