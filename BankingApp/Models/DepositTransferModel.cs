namespace BankingApp.Models
{
    public class DepositTransferModel
    {
        public string SourceAccountNumber { get; set; } = default!;
        public string DestinationAccountNumber { get; set; } = default!;
        public decimal Amount { get; set; }
    }
}