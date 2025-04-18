using FluentValidation;

namespace Application.Features.Transactions.Deposit;

public class DepositTransactionCommandValidator : AbstractValidator<DepositTransactionCommand>
{
    public DepositTransactionCommandValidator()
    {
        RuleFor(depositTransactionCommand => depositTransactionCommand.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .WithMessage("Account ID must be a valid GUID.");
        
        RuleFor(depositTransactionCommand => depositTransactionCommand.Amount)
            .NotNull()
            .WithMessage("Amount should not be null.")
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.")
            .LessThan(1000000M)
            .WithMessage("Amount must be less than 1 billion.")
            .PrecisionScale(18, 2, true)
            .WithMessage("Amount must have a maximum of 2 decimal places and a maximum of 18 digits in total.")
            .Must(amount => !double.IsInfinity((double)amount))
            .WithMessage("Amount must be a finite number.")
            .Must(amount => !double.IsNaN((double)amount))
            .WithMessage("Amount must be a valid number.");
    }
}
