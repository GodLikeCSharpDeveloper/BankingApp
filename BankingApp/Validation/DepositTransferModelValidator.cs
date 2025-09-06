using BankingApp.Models;
using FluentValidation;

namespace BankingApp.Validation
{
    public class DepositTransferModelValidator : AbstractValidator<DepositTransferModel>
    {
        public DepositTransferModelValidator()
        {
            RuleFor(x => x.SourceAccountNumber)
                .NotEmpty();

            RuleFor(x => x.DestinationAccountNumber)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Transfer amount must be greater than zero");

            RuleFor(x => x)
                .Must(x => x.SourceAccountNumber != x.DestinationAccountNumber)
                .WithMessage("Source and destination account numbers must be different.");
        }
    }
}