namespace BankingApp.Models
{
    public class DepositModel
    {
        public string AccountNumber { get; set; } = default!;
        public decimal Amount { get; set; }
    }
}