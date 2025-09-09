using BankingApp.Configurations;
using BankingApp.Services.Transaction;
using System.Data;

namespace BankingAppTest.Services.Funds
{
    public class FakeTransactionService(AppDbContext context) : ITransactionService
    {
        private readonly AppDbContext _context = context;

        public Task ExecuteAsync(Func<AppDbContext, Task> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken token = default)
        {
            return func(_context);
        }
    }
}