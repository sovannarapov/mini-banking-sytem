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

internal sealed class WithdrawTransactionCommandHandler(
    IApplicationDbContext context,
    TimeProvider timeProvider,
    ILogger logger)
    : ICommandHandler<WithdrawTransactionCommand, TransactionResponse>
{
    public async Task<Result<TransactionResponse>> Handle(WithdrawTransactionCommand transactionCommand,
        CancellationToken cancellationToken)
    {
        switch (transactionCommand.Amount)
        {
            case <= TransactionConstants.MinTransactionAmount:
                logger.Warning("Invalid withdrawal amount: {Amount}", transactionCommand.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.InvalidAmount(transactionCommand.Amount));
            case > TransactionConstants.MaxWithdrawAmount:
                logger.Warning("Withdrawal amount exceeds maximum limit: {Amount}", transactionCommand.Amount);
                return Result.Failure<TransactionResponse>(
                    TransactionError.ExceedsMaximumLimit(transactionCommand.Amount));
        }

        Account? account =
            await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == transactionCommand.AccountId,
                cancellationToken);

        if (account == null)
        {
            return Result.Failure<TransactionResponse>(AccountError.NotFound(transactionCommand.AccountId));
        }

        if (account.Balance < transactionCommand.Amount)
        {
            logger.Warning("Insufficient funds: Account {AccountId}, Balance {Balance}, Withdrawal Amount {Amount}",
                account.Id, account.Balance, transactionCommand.Amount);
            return Result.Failure<TransactionResponse>(
                TransactionError.InsufficientFunds(account.Balance, transactionCommand.Amount));
        }

        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var withdrawTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = transactionCommand.AccountId,
                Type = TransactionType.Withdraw,
                Amount = transactionCommand.Amount,
                TargetAccountNumber = account.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            account.Balance -= transactionCommand.Amount;
            context.Transactions.Add(withdrawTransaction);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.Information(
                "Withdrawal successful: Account {AccountId}, Amount {Amount}, Remaining Balance {Balance}",
                account.Id,
                transactionCommand.Amount,
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
                transactionCommand.AccountId,
                ex.Message);

            await transaction.RollbackAsync(cancellationToken);

            return Result.Failure<TransactionResponse>(TransactionError.Failed(ex.Message));
        }
    }
}
