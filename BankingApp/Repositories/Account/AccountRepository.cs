using BankingApp.Configurations;
using BankingApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories.Account
{
    public class AccountRepository(AppDbContext context) : EfRepository<AccountEntity>(context), IAccountRepository
    {
        public Task<AccountEntity?> FindTrackedByAccountNumberAsync(string accountNumber)
        {
            return AsQueryable().FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public Task<AccountEntity?> FindByAccountNumberAsync(string accountNumber)
        {
            return AsQueryable().AsNoTracking().FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }
    }
}