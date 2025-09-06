namespace BankingApp.Models
{
    public class UpdateAccountDto
    {
        public string AccountNumber { get; init; } = default!;
        public decimal Balance { get; set; }
    }
}