using BankingApp.Configurations;
using BankingApp.Entities;
using BankingApp.Exceptions;
using BankingApp.Models;
using BankingApp.Services.Funds;
using Microsoft.EntityFrameworkCore;

namespace BankingAppTest.Services.Funds
{
    public class FundsServiceTests
    {
        private readonly AppDbContext _context;
        private readonly FundsService _fundsService;
        private readonly FakeTransactionService _fakeTransactionService;

        public FundsServiceTests()
        {
            _context = DbHelper.GetContext();
            _fakeTransactionService = new(_context);
            _fundsService = new(_fakeTransactionService);
        }

        [Fact]
        public async Task DepositAsync_IncreasesBalance()
        {
            // Arrange
            _context.Accounts.Add(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 100
            });
            await _context.SaveChangesAsync();

            // Act
            await _fundsService.DepositAsync(new DepositModel
            {
                AccountNumber = "ACC123",
                Amount = 50
            });

            // Assert
            var account = await _context.Accounts.FirstAsync(a => a.AccountNumber == "ACC123");
            Assert.Equal(150, account.Balance);
        }

        [Fact]
        public async Task WithdrawAsync_DecreasesBalance()
        {
            // Arrange
            _context.Accounts.Add(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 100
            });
            await _context.SaveChangesAsync();

            // Act
            await _fundsService.WithdrawAsync(new DepositModel
            {
                AccountNumber = "ACC123",
                Amount = 40
            });

            // Assert
            var account = await _context.Accounts.FirstAsync(a => a.AccountNumber == "ACC123");
            Assert.Equal(60, account.Balance);
        }

        [Fact]
        public async Task TransferAsync_MovesFundsBetweenAccounts()
        {
            // Arrange
            _context.Accounts.AddRange(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 200
            },
            new AccountEntity
            {
                AccountNumber = "ACC456",
                Balance = 100
            });
            await _context.SaveChangesAsync();

            // Act
            await _fundsService.TransferAsync(new DepositTransferModel
            {
                SourceAccountNumber = "ACC123",
                DestinationAccountNumber = "ACC456",
                Amount = 70
            });

            // Assert
            var fromAccount = await _context.Accounts.FirstAsync(a => a.AccountNumber == "ACC123");
            var toAccount = await _context.Accounts.FirstAsync(a => a.AccountNumber == "ACC456");
            Assert.Equal(130, fromAccount.Balance);
            Assert.Equal(170, toAccount.Balance);
        }

        [Fact]
        public async Task WithdrawAsync_ThrowsOnInsufficientFunds()
        {
            // Arrange
            _context.Accounts.Add(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 30
            });
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InsufficientFundsException>(() => _fundsService.WithdrawAsync(new DepositModel
            {
                AccountNumber = "ACC123",
                Amount = 50
            }));
        }

        [Fact]
        public async Task TransferAsync_ThrowsOnInsufficientFunds()
        {
            // Arrange
            _context.Accounts.AddRange(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 30
            },
            new AccountEntity
            {
                AccountNumber = "ACC456",
                Balance = 100
            });
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InsufficientFundsException>(() => _fundsService.TransferAsync(new DepositTransferModel
            {
                SourceAccountNumber = "ACC123",
                DestinationAccountNumber = "ACC456",
                Amount = 50
            }));
        }

        [Fact]
        public async Task DepositAsync_ThrowsOnAccountNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _fundsService.DepositAsync(new DepositModel
            {
                AccountNumber = "WrongAccountNumber",
                Amount = 50
            }));
        }

        [Fact]
        public async Task WithdrawAsync_ThrowsOnAccountNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _fundsService.WithdrawAsync(new DepositModel
            {
                AccountNumber = "WrongAccountNumber",
                Amount = 50
            }));
        }

        [Fact]
        public async Task TransferAsync_ThrowsOnAccountNotFound()
        {
            // Arrange
            _context.Accounts.Add(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 100
            });
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<AccountNotFoundException>(() => _fundsService.TransferAsync(new DepositTransferModel
            {
                SourceAccountNumber = "ACC123",
                DestinationAccountNumber = "WrongAccountNumber",
                Amount = 50
            }));
        }

        [Fact]
        public async Task DepositAsync_CancelOperation()
        {
            // Arrange
            _context.Accounts.Add(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 100
            });
            await _context.SaveChangesAsync();
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => _fundsService.DepositAsync(new DepositModel
            {
                AccountNumber = "ACC123",
                Amount = 50
            }, cts.Token));
        }

        [Fact]
        public async Task WithdrawAsync_CancelOperation()
        {
            // Arrange
            _context.Accounts.Add(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 100
            });
            await _context.SaveChangesAsync();
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => _fundsService.WithdrawAsync(new DepositModel
            {
                AccountNumber = "ACC123",
                Amount = 50
            }, cts.Token));
        }

        [Fact]
        public async Task TransferAsync_CancelOperation()
        {
            // Arrange
            _context.Accounts.AddRange(new AccountEntity
            {
                AccountNumber = "ACC123",
                Balance = 200
            },
            new AccountEntity
            {
                AccountNumber = "ACC456",
                Balance = 100
            });
            await _context.SaveChangesAsync();
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => _fundsService.TransferAsync(new DepositTransferModel
            {
                SourceAccountNumber = "ACC123",
                DestinationAccountNumber = "ACC456",
                Amount = 70
            }, cts.Token));
        }
    }
}