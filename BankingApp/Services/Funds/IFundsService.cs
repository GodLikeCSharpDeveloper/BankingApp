using BankingApp.Models;

namespace BankingApp.Services.Account
{
    public interface IFundsService
    {
        Task DepositAsync(DepositModel depositModel);
        Task WithdrawAsync(DepositModel depositModel);
        Task TransferAsync(DepositTransferModel depositTransferModel);
    }
}