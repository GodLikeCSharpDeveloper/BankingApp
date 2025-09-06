using BankingApp.Configurations;
using BankingApp.Entities;
using BankingApp.Exceptions;
using BankingApp.Models;
using BankingApp.Services.Transaction;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BankingApp.Services.Account
{
    public class FundsService(ITransactionService transactionService) : IFundsService
    {
        public Task DepositAsync(DepositModel depositModel)
        {
            return transactionService.ExecuteAsync(async db =>
            {
                var account = await GetByAccountNumberAsync(db, depositModel.AccountNumber);

                account.Balance += depositModel.Amount;
            }, IsolationLevel.Serializable);
        }

        public Task WithdrawAsync(DepositModel depositModel)
        {
            return transactionService.ExecuteAsync(async db =>
            {
                var account = await GetByAccountNumberAsync(db, depositModel.AccountNumber);

                if (account.Balance < depositModel.Amount)
                    throw new InsufficientFundsException();

                account.Balance -= depositModel.Amount;
            }, IsolationLevel.Serializable);
        }

        public Task TransferAsync(DepositTransferModel depositTransferModel)
        {
            return transactionService.ExecuteAsync(async db =>
            {
                var from = await GetByAccountNumberAsync(db, depositTransferModel.SourceAccountNumber);
                var to = await GetByAccountNumberAsync(db, depositTransferModel.DestinationAccountNumber);

                if (from.Balance < depositTransferModel.Amount)
                    throw new InsufficientFundsException();

                from.Balance -= depositTransferModel.Amount;
                to.Balance += depositTransferModel.Amount;
            }, IsolationLevel.Serializable);
        }

        private static async Task<AccountEntity> GetByAccountNumberAsync(AppDbContext db, string accountNumber)
        {
            var account = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber)
                ?? throw new AccountNotFoundException("Account not found.");
            return account;
        }
    }
}