using Application.Abstractions.Messaging;
using Application.Common.Interfaces;
using Application.Dtos.Transaction;
using Domain.Accounts;
using Shared;

namespace Application.Features.Transactions.Deposit;

public sealed class DepositTransactionCommandHandler(
    IValidationService validationService,
    IAccountService accountService,
    ITransactionService transactionService)
    : ICommandHandler<DepositTransactionCommand, TransactionResponse>
{
    public async Task<Result<TransactionResponse>> Handle(DepositTransactionCommand command,
        CancellationToken cancellationToken)
    {
        Account? account = await accountService.GetAccountByIdAsync(command.AccountId, cancellationToken);

        if (account is null)
        {
            return Result.Failure<TransactionResponse>(AccountError.NotFound(accountId: command.AccountId));
        }

        Result validationResult = validationService.ValidateDepositAmount(command.Amount);

        if (validationResult.IsFailure)
        {
            return Result.Failure<TransactionResponse>(validationResult.Error);
        }

        accountService.Deposit(account, command.Amount);

        return await transactionService.ProcessDepositAsync(account, command.Amount, cancellationToken);
    }
}
