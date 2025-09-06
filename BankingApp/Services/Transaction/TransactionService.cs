using BankingApp.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BankingApp.Services.Transaction
{
    public class TransactionService(AppDbContext dbContext) : ITransactionService
    {
        public async Task ExecuteAsync(Func<AppDbContext, Task> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken token = default)
        {
            await using var tx = await dbContext.Database.BeginTransactionAsync(isolationLevel, token);
            try
            {
                await func(dbContext);
                await dbContext.SaveChangesAsync(token);
                await tx.CommitAsync(token);
            }
            catch
            {
                await tx.RollbackAsync(token);
                throw;
            }
        }
    }
}