using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;
using Domain.Accounts;
using Domain.Constants;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Shared;

namespace Application.Features.Transactions.Transfer;

internal sealed class TransferTransactionCommandHandler(
    IApplicationDbContext context,
    TimeProvider timeProvider,
    ILogger logger)
    : ICommandHandler<TransferTransactionCommand, TransferResponse>
{
    public async Task<Result<TransferResponse>> Handle(TransferTransactionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Amount <= TransactionConstants.MinTransactionAmount)
        {
            logger.Warning("Amount must be greater than {Amount}.", command.Amount);
            return Result.Failure<TransferResponse>(TransactionError.InvalidAmount(command.Amount));
        }

        Account? sourceAccount =
            await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == command.AccountId,
                cancellationToken);

        if (sourceAccount == null)
        {
            return Result.Failure<TransferResponse>(AccountError.NotFound(accountId: command.AccountId));
        }

        Account? targetAccount =
            await context.Accounts.FirstOrDefaultAsync(
                acc => acc.AccountNumber == command.TargetAccountNumber, cancellationToken);

        if (targetAccount == null)
        {
            return Result.Failure<TransferResponse>(AccountError.NotFound(accountNumber: command.TargetAccountNumber));
        }

        if (sourceAccount.Balance < command.Amount)
        {
            logger.Warning("Insufficient funds: Account {AccountId}, Balance {Balance}, Withdrawal Amount {Amount}",
                sourceAccount.Id, sourceAccount.Balance, command.Amount);
            return Result.Failure<TransferResponse>(
                TransactionError.InsufficientBalance(sourceAccount.Balance, command.Amount));
        }

        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var debitTransactionId = Guid.NewGuid();
            var creditTransactionId = Guid.NewGuid();

            var debitTransaction = new Transaction
            {
                Id = debitTransactionId,
                AccountId = sourceAccount.Id,
                Type = TransactionType.Transfer,
                Amount = -command.Amount,
                TargetAccountNumber = targetAccount.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            sourceAccount.Balance -= command.Amount;
            context.Transactions.Add(debitTransaction);

            var creditTransaction = new Transaction
            {
                Id = creditTransactionId,
                AccountId = targetAccount.Id,
                Type = TransactionType.Transfer,
                Amount = command.Amount,
                TargetAccountNumber = sourceAccount.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            targetAccount.Balance += command.Amount;
            context.Transactions.Add(creditTransaction);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.Information(
                "Withdrawal successful: Account {AccountId}, Amount {Amount}, Remaining Balance {Balance}",
                sourceAccount.Id,
                command.Amount,
                sourceAccount.Balance);

            return Result.Success(new TransferResponse(creditTransactionId, debitTransactionId));
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Failed to process withdrawal for sourceAccount {AccountId}: {Message}",
                command.AccountId,
                ex.Message);

            await transaction.RollbackAsync(cancellationToken);

            return Result.Failure<TransferResponse>(TransactionError.Failed(ex.Message));
        }
    }
}
