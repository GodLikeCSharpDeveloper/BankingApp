using BankingApp.Controllers;
using BankingApp.Models;
using BankingApp.Services.Funds;
using BankingApp.Services.Queue;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankingAppTest.Controllers
{
    public class FundsControllerTests
    {
        private readonly Mock<IFundsService> fundsServiceMock;
        private readonly Mock<IOperationQueue> operationQueueMock;
        private readonly Mock<IValidator<DepositTransferModel>> validatorMock;
        private readonly FundsController _fundsController;
        public FundsControllerTests()
        {
            fundsServiceMock = new Mock<IFundsService>();
            operationQueueMock = new Mock<IOperationQueue>();
            validatorMock = new Mock<IValidator<DepositTransferModel>>();
            _fundsController = new FundsController(fundsServiceMock.Object, operationQueueMock.Object, validatorMock.Object);
        }

        [Fact]
        public async Task Deposit_ValidModel_ReturnsOk()
        {
            // Arrange
            var model = new DepositModel { AccountNumber = "ACC000001", Amount = 100 };

            // Act
            var result = await _fundsController.Deposit(model);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            operationQueueMock.Verify(q => q.EnqueueAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Transfer_ValidModel_ReturnsOk()
        {
            // Arrange
            var model = new DepositTransferModel { SourceAccountNumber = "ACC000001", DestinationAccountNumber = "ACC000002", Amount = 75 };
            validatorMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(new ValidationResult());

            // Act
            var result = await _fundsController.Transfer(model);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            operationQueueMock.Verify(q => q.EnqueueAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Withdraw_ValidModel_ReturnsOk()
        {
            // Arrange
            var model = new DepositModel { AccountNumber = "ACC000001", Amount = 50 };

            // Act
            var result = await _fundsController.Withdraw(model);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            operationQueueMock.Verify(q => q.EnqueueAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deposit_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var model = new DepositModel { AccountNumber = "", Amount = -100 };
            _fundsController.ModelState.AddModelError("Amount", "Invalid");
            _fundsController.ModelState.AddModelError("AccountNumber", "Required");

            // Act
            var result = await _fundsController.Deposit(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            operationQueueMock.Verify(q => q.EnqueueAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Transfer_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var model = new DepositTransferModel { SourceAccountNumber = "ACC000001", DestinationAccountNumber = "ACC000001", Amount = 0 };
            var validationResult = new ValidationResult(
            [
                new ValidationFailure("DestinationAccountNumber", "Destination account must be different from source account."),
                new ValidationFailure("Amount", "Amount must be greater than zero.")
            ]);
            validatorMock.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(validationResult);

            // Act
            var result = await _fundsController.Transfer(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            operationQueueMock.Verify(q => q.EnqueueAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Withdraw_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var model = new DepositModel { AccountNumber = "", Amount = -50 };
            _fundsController.ModelState.AddModelError("Amount", "Invalid");
            _fundsController.ModelState.AddModelError("AccountNumber", "Required");

            // Act
            var result = await _fundsController.Withdraw(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            operationQueueMock.Verify(q => q.EnqueueAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}