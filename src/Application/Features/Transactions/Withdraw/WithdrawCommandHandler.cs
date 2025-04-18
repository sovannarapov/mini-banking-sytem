using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;
using Domain.Accounts;
using Domain.Constants;
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
    public async Task<Result<TransactionResponse>> Handle(WithdrawCommand command, CancellationToken cancellationToken)
    {
        switch (command.Amount)
        {
            case <= TransactionConstants.MinTransactionAmount:
                logger.Warning("Invalid withdrawal amount: {Amount}", command.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.InvalidAmount(command.Amount));
            case > TransactionConstants.MaxWithdrawAmount:
                logger.Warning("Withdrawal amount exceeds maximum limit: {Amount}", command.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.ExceedsMaximumLimit(command.Amount));
        }

        Account? account =
            await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == command.AccountId, cancellationToken);

        if (account == null)
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

        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var withdrawTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = command.AccountId,
                Type = TransactionType.Withdraw,
                Amount = command.Amount,
                TargetAccountNumber = account.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            account.Balance -= command.Amount;
            context.Transactions.Add(withdrawTransaction);

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
