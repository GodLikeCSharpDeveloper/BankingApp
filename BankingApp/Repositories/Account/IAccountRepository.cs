using BankingApp.Entities;

namespace BankingApp.Repositories.Account
{
    public interface IAccountRepository : IGenericRepository<AccountEntity>
    {
        Task<AccountEntity?> FindByAccountNumberAsync(string accountNumber);
        Task<AccountEntity?> FindTrackedByAccountNumberAsync(string accountNumber);
    }
}