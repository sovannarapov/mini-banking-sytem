using Domain.Accounts;
using Shared;

namespace Application.Common.Interfaces;

public interface IAccountService
{
    Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken);
    void Deposit(Account account, decimal amount);
    Result Withdraw(Account account, decimal amount);
}
