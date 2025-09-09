using BankingApp.Models;

namespace BankingApp.Services.Funds
{
    public interface IFundsService
    {
        Task DepositAsync(DepositModel depositModel, CancellationToken cancellationToken = default);
        Task WithdrawAsync(DepositModel depositModel, CancellationToken cancellationToken = default);
        Task TransferAsync(DepositTransferModel depositTransferModel, CancellationToken cancellationToken = default);
    }
}