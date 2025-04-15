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

namespace Application.Features.Transactions.Deposit;

internal sealed class DepositCommandHandler(IApplicationDbContext context, TimeProvider timeProvider, ILogger logger)
    : ICommandHandler<DepositCommand, TransactionResponse>
{
    private const decimal MaxDepositAmount = 1000000M;

    public async Task<Result<TransactionResponse>> Handle(DepositCommand command, CancellationToken cancellationToken)
    {
        switch (command.Amount)
        {
            case <= 0:
                logger.Warning("Invalid deposit amount: {Amount}", command.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.InvalidAmount(command.Amount));
            case > MaxDepositAmount:
                logger.Warning("Deposit amount exceeds maximum limit: {Amount}", command.Amount);
                return Result.Failure<TransactionResponse>(TransactionError.ExceedsMaximumLimit(command.Amount));
        }

        Account? account =
            await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == command.AccountId, cancellationToken);

        if (account is null)
        {
            return Result.Failure<TransactionResponse>(AccountError.NotFound(command.AccountId));
        }

        IDbContextTransaction? transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var depositTransaction = new Transaction
            {
                AccountId = command.AccountId,
                Type = TransactionType.Deposit,
                Amount = command.Amount,
                TargetAccountNumber = account.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            context.Transactions.Add(depositTransaction);

            account.Balance += command.Amount;
            context.Accounts.Update(account);

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
