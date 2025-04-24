using Application.Dtos.Transaction;
using Domain.Accounts;
using Shared;

namespace Application.Common.Interfaces;

public interface ITransactionService
{
    Task<Result<TransactionResponse>> ProcessDepositAsync(Account account, decimal amount,
        CancellationToken cancellationToken);
}
