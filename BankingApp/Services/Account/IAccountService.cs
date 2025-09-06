using BankingApp.Models;

namespace BankingApp.Services.Account
{
    public interface IAccountService
    {
        Task<GetAccountDto> CreateAsync(CreateAccountDto createAccountDto);
        Task<List<GetAccountDto>> GetAllAsync();
        Task<GetAccountDto?> FindByAccountNumberAsync(string accountNumber);
    }
}