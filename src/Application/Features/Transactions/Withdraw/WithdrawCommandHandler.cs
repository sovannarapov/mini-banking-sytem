using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;
using Domain.Accounts;
using Domain.Extensions;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Shared;

namespace Application.Features.Transactions.Withdraw;

internal sealed class WithdrawCommandHandler(IApplicationDbContext context, TimeProvider timeProvider, ILogger logger)
    : ICommandHandler<WithdrawCommand, TransactionResponse>
{
    private const decimal MaxWithdrawAmount = 100000M;

    public async Task<Result<TransactionResponse>> Handle(WithdrawCommand command, CancellationToken cancellationToken)
    {
        switch (command.Amount)
        {
            case <= 0:
                logger.Warning("Invalid withdrawal amount: {Amount}", command.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.InvalidAmount(command.Amount));
            case > MaxWithdrawAmount:
                logger.Warning("Withdrawal amount exceeds maximum limit: {Amount}", command.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.ExceedsMaximumLimit(command.Amount));
        }

        Account? account =
            await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == command.AccountId, cancellationToken);

        if (account is null)
        {
            return Result.Failure<TransactionResponse>(AccountError.NotFound(command.AccountId));
        }

        if (account.Balance < command.Amount)
        {
            logger.Warning("Insufficient funds: Account {AccountId}, Balance {Balance}, Withdrawal Amount {Amount}",
                account.Id, account.Balance, command.Amount);
            return Result.Failure<TransactionResponse>(
                TransactionError.InsufficientFunds(account.Balance, command.Amount));
        }

        IDbContextTransaction? transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var withdrawTransaction = new Transaction
            {
                AccountId = command.AccountId,
                Type = TransactionType.Withdraw,
                Amount = command.Amount,
                TargetAccountNumber = account.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            context.Transactions.Add(withdrawTransaction);

            account.Balance -= command.Amount;
            context.Accounts.Update(account);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.Information(
                "Withdrawal successful: Account {AccountId}, Amount {Amount}, Remaining Balance {Balance}",
                account.Id,
                command.Amount,
                account.Balance);

            var response = new TransactionResponse(
                withdrawTransaction.Id,
                withdrawTransaction.AccountId,
                withdrawTransaction.Type.GetDisplayName(),
                withdrawTransaction.Amount,
                withdrawTransaction.TargetAccountNumber,
                withdrawTransaction.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Failed to process withdrawal for account {AccountId}: {Message}",
                command.AccountId,
                ex.Message);

            await transaction.RollbackAsync(cancellationToken);

            return Result.Failure<TransactionResponse>(TransactionError.Failed(ex.Message));
        }
    }
}
