using Application.Features.Accounts.Create;
using FluentValidation;

namespace Application.Features.Accounts.Create;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(c => c.OwnerName)
            .NotNull()
            .WithMessage("Owner name cannot be null.")
            .NotEmpty()
            .WithMessage("Owner name is required.")
            .MinimumLength(2)
            .WithMessage("Owner name must be at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("Owner name must not exceed 100 characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Owner name cannot contain only whitespace.")
            .Matches("^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$")
            .WithMessage("Owner name can only contain letters, spaces, and characters: ' , . -")
            .Must(name => name.Trim().Length >= 2)
            .WithMessage("Owner name must contain at least 2 non-whitespace characters.");


        RuleFor(c => c.AccountType)
            .NotEmpty()
            .WithMessage("Account type is required.")
            .IsInEnum()
            .WithMessage("Account type must be either Savings, Checking or Business.");
    }
}
