using Shared;

namespace Domain.Accounts;

public static class AccountError
{
    public static Error NotFound(Guid? accountId = null, string? accountNumber = null)
    {
        if (accountId is null && accountNumber is null)
        {
            return Error.NotFound($"{nameof(Account)}.NotFound", "Account was not found.");
        }

        return accountId.HasValue
            ? Error.NotFound($"{nameof(Account)}.NotFound", $"Account with ID '{accountId}' was not found.")
            : Error.NotFound($"{nameof(Account)}.NotFound", $"Account with number '{accountNumber}' was not found.");
    }

    public static Error Required(string fieldName) => Error.Failure(
        $"{nameof(Account)}.Required", 
        $"{fieldName} is required.");
}
