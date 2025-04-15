using Domain.Accounts;

namespace Application.Dtos.Account;

public sealed class CreateAccountRequest
{
    public string OwnerName { get; set; }
    public AccountType AccountType { get; set; } = AccountType.Savings;
}
