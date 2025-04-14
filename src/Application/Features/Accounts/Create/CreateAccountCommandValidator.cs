using Application.Features.Accounts.Create;
using FluentValidation;

namespace Application.Features.Accounts.Create;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(c => c.OwnerName)
            .NotEmpty()
            .WithMessage("Owner name is required.")
            .MaximumLength(100)
            .WithMessage("Owner name must not exceed 100 characters.");
        RuleFor(c => c.AccountType)
            .NotEmpty()
            .WithMessage("Account type is required.")
            .IsInEnum()
            .WithMessage("Account type must be either Savings, Checking or Business.");
    }
}
