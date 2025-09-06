using BankingApp.Configurations;
using System.Data;

namespace BankingApp.Services.Transaction
{
    public interface ITransactionService
    {
        Task ExecuteAsync(Func<AppDbContext, Task> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken token = default);
    }
}