using BankingApp.Repositories;

namespace BankingApp.Entities
{
    public class AccountEntity : IEntity
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = default!;
        public decimal Balance { get; set; }
    }
}