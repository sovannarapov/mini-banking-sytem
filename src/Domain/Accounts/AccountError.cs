using Shared;

namespace Domain.Accounts;

public static class AccountError
{
    public static Error NotFound(Guid? accountId = null, string? accountNumber = null)
    {
        if (accountId is null && accountNumber is null)
        {
            return Error.NotFound("Accounts.NotFound", "Account was not found.");
        }

        return accountId.HasValue
            ? Error.NotFound("Accounts.NotFound", $"Account with ID '{accountId}' was not found.")
            : Error.NotFound("Accounts.NotFound", $"Account with number '{accountNumber}' was not found.");
    }
}
