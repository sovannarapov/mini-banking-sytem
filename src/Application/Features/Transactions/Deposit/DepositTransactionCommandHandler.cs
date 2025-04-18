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

namespace Application.Features.Transactions.Deposit;

internal sealed class DepositTransactionCommandHandler(IApplicationDbContext context, TimeProvider timeProvider, ILogger logger)
    : ICommandHandler<DepositTransactionCommand, TransactionResponse>
{
    public async Task<Result<TransactionResponse>> Handle(DepositTransactionCommand transactionCommand, CancellationToken cancellationToken)
    {
        switch (transactionCommand.Amount)
        {
            case <= TransactionConstants.MinTransactionAmount:
                logger.Warning("Invalid deposit amount: {Amount}", transactionCommand.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.InvalidAmount(transactionCommand.Amount));
            case > TransactionConstants.MaxDepositAmount:
                logger.Warning("Deposit amount exceeds maximum limit: {Amount}", transactionCommand.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.ExceedsMaximumLimit(transactionCommand.Amount));
        }

        Account? account =
            await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == transactionCommand.AccountId, cancellationToken);

        if (account == null)
        {
            return Result.Failure<TransactionResponse>(AccountError.NotFound(transactionCommand.AccountId));
        }

        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var depositTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = transactionCommand.AccountId,
                Type = TransactionType.Deposit,
                Amount = transactionCommand.Amount,
                TargetAccountNumber = account.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            account.Balance += transactionCommand.Amount;
            context.Transactions.Add(depositTransaction);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.Information(
                "Deposit successful: Account {AccountId}, Amount {Amount}",
                account.Id,
                transactionCommand.Amount);

            var response = new TransactionResponse(
                depositTransaction.Id,
                depositTransaction.AccountId,
                depositTransaction.Type.GetDisplayName(),
                depositTransaction.Amount,
                depositTransaction.TargetAccountNumber,
                depositTransaction.CreatedAt
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Failed to process deposit for account {AccountId}: {Message}",
                transactionCommand.AccountId,
                ex.Message);

            await transaction.RollbackAsync(cancellationToken);

            return Result.Failure<TransactionResponse>(TransactionError.Failed(ex.Message));
        }
    }
}
