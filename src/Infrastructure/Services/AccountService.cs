using Application.Abstractions.Data;
using Application.Common.Interfaces;
using Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Infrastructure.Services;

public class AccountService(IApplicationDbContext context) : IAccountService
{
    public async Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        return await context.Accounts.FirstOrDefaultAsync(account => account.Id == accountId, cancellationToken);
    }

    public void Deposit(Account account, decimal amount) => account.Balance += amount;

    public Result Withdraw(Account account, decimal amount)
    {
        decimal newBalance = account.Balance - amount;

        account.Balance = newBalance;

        return Result.Success();
    }
}
