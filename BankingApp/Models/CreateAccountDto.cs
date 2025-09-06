using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class CreateAccountDto
    {
        [Required]
        [Range(typeof(decimal), "1", "79228162514264337593543950335")]
        public decimal InitialBalance { get; init; }
    }
}