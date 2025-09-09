using BankingApp.Configurations;
using BankingApp.Entities;
using BankingApp.Exceptions;
using BankingApp.Models;
using BankingApp.Services.Transaction;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BankingApp.Services.Funds
{
    public class FundsService(ITransactionService transactionService) : IFundsService
    {
        public Task DepositAsync(DepositModel depositModel, CancellationToken cancellationToken = default)
        {
            return transactionService.ExecuteAsync(async db =>
            {
                var account = await GetByAccountNumberAsync(db, depositModel.AccountNumber, cancellationToken);

                account.Balance += depositModel.Amount;
            }, IsolationLevel.Serializable, cancellationToken);
        }

        public Task WithdrawAsync(DepositModel depositModel, CancellationToken cancellationToken = default)
        {
            return transactionService.ExecuteAsync(async db =>
            {
                var account = await GetByAccountNumberAsync(db, depositModel.AccountNumber, cancellationToken);

                if (account.Balance < depositModel.Amount)
                    throw new InsufficientFundsException();

                account.Balance -= depositModel.Amount;
            }, IsolationLevel.Serializable, cancellationToken);
        }

        public Task TransferAsync(DepositTransferModel depositTransferModel, CancellationToken cancellationToken = default)
        {
            return transactionService.ExecuteAsync(async db =>
            {
                var from = await GetByAccountNumberAsync(db, depositTransferModel.SourceAccountNumber, cancellationToken);
                var to = await GetByAccountNumberAsync(db, depositTransferModel.DestinationAccountNumber, cancellationToken);

                if (from.Balance < depositTransferModel.Amount)
                    throw new InsufficientFundsException();

                from.Balance -= depositTransferModel.Amount;
                to.Balance += depositTransferModel.Amount;
            }, IsolationLevel.Serializable, cancellationToken);
        }

        private static async Task<AccountEntity> GetByAccountNumberAsync(AppDbContext db, string accountNumber, CancellationToken cancellationToken)
        {
            var account = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken)
                ?? throw new AccountNotFoundException("Account not found.");
            return account;
        }
    }
}