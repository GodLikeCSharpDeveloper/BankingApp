namespace BankingApp.Models
{
    public class GetAccountDto
    {
        public string AccountNumber { get; set; } = default!;
        public decimal Balance { get; set; }
    }
}