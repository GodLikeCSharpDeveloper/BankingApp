namespace BankingApp.Services.Account
{
    public interface IAccountNumberGenerator
    {
        Task<string> GenerateAsync();
    }
}