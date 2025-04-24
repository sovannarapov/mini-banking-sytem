using Domain.Accounts;

namespace Application.Common.Interfaces;

public interface IAccountService
{
    Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken);
    void UpdateBalance(Account account, decimal amount);
}
