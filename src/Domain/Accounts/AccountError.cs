using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared;

namespace Domain.Accounts;

public static class AccountError
{
    public static Error NotFound(Guid accountId) => Error.NotFound(
        "Accounts.NotFound",
        $"The account with id {accountId} was not found.");
}
