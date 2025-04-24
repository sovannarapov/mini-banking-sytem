using Application.Abstractions.Data;
using Application.Common.Interfaces;
using Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AccountService(IApplicationDbContext context) : IAccountService
{
    public async Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        return await context.Accounts.FirstOrDefaultAsync(account => account.Id == accountId, cancellationToken);
    }

    public void UpdateBalance(Account account, decimal amount)
    {
        account.Balance += amount;
    }
}
