using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class DepositTransferModel
    {
        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string SourceAccountNumber { get; set; } = default!;

        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string DestinationAccountNumber { get; set; } = default!;

        [Range(typeof(decimal), "1", "79228162514264337593543950335")]
        public decimal Amount { get; set; }
    }
}