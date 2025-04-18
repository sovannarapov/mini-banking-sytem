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

internal sealed class DepositCommandHandler(IApplicationDbContext context, TimeProvider timeProvider, ILogger logger)
    : ICommandHandler<DepositCommand, TransactionResponse>
{
    public async Task<Result<TransactionResponse>> Handle(DepositCommand command, CancellationToken cancellationToken)
    {
        switch (command.Amount)
        {
            case <= TransactionConstants.MinTransactionAmount:
                logger.Warning("Invalid deposit amount: {Amount}", command.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.InvalidAmount(command.Amount));
            case > TransactionConstants.MaxDepositAmount:
                logger.Warning("Deposit amount exceeds maximum limit: {Amount}", command.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.ExceedsMaximumLimit(command.Amount));
        }

        Account? account =
            await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == command.AccountId, cancellationToken);

        if (account == null)
        {
            return Result.Failure<TransactionResponse>(AccountError.NotFound(command.AccountId));
        }

        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var depositTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = command.AccountId,
                Type = TransactionType.Deposit,
                Amount = command.Amount,
                TargetAccountNumber = account.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            account.Balance += command.Amount;
            context.Transactions.Add(depositTransaction);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.Information(
                "Deposit successful: Account {AccountId}, Amount {Amount}",
                account.Id,
                command.Amount);

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
                command.AccountId,
                ex.Message);

            await transaction.RollbackAsync(cancellationToken);

            return Result.Failure<TransactionResponse>(TransactionError.Failed(ex.Message));
        }
    }
}
