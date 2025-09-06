namespace BankingApp.Services.Funds
{
    public interface IOperationQueue
    {
        Task EnqueueAsync(Func<Task> operation);
    }
}