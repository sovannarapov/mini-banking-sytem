using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;
using Domain.Accounts;
using Domain.Extensions;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog.Core;
using Shared;

namespace Application.Features.Transactions.Transfer;

internal sealed class TransferCommandHandler(IApplicationDbContext context, TimeProvider timeProvider, Logger logger)
    : ICommandHandler<TransferTransactionCommand, TransactionResponse>
{
    private const decimal MaxWithdrawAmount = 100000M;

    public async Task<Result<TransactionResponse>> Handle(TransferTransactionCommand transactionCommand, CancellationToken cancellationToken)
    {
        switch (transactionCommand.Amount)
        {
            case <= 0:
                logger.Warning("Invalid withdrawal amount: {Amount}", transactionCommand.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.InvalidAmount(transactionCommand.Amount));
            case > MaxWithdrawAmount:
                logger.Warning("Withdrawal amount exceeds maximum limit: {Amount}", transactionCommand.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.ExceedsMaximumLimit(transactionCommand.Amount));
        }

        Account? account =
            await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == transactionCommand.AccountId, cancellationToken);

        if (account is null)
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
            var transferTransaction = new Transaction
            {
                AccountId = transactionCommand.AccountId,
                Type = TransactionType.Transfer,
                Amount = -transactionCommand.Amount,
                TargetAccountNumber = account.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            context.Transactions.Add(transferTransaction);

            account.Balance += transactionCommand.Amount;
            context.Accounts.Update(account);
            
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.Information(
                "Withdrawal successful: Account {AccountId}, Amount {Amount}, Remaining Balance {Balance}",
                account.Id,
                transactionCommand.Amount,
                account.Balance);

            var response = new TransactionResponse(
                transferTransaction.Id,
                transferTransaction.AccountId,
                transferTransaction.Type.GetDisplayName(),
                transferTransaction.Amount,
                transferTransaction.TargetAccountNumber,
                transferTransaction.CreatedAt
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
