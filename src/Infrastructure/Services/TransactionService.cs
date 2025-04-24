using Application.Abstractions.Data;
using Application.Common.Interfaces;
using Application.Dtos.Transaction;
using Domain.Accounts;
using Domain.Extensions;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Shared;

namespace Infrastructure.Services;

public class TransactionService(IApplicationDbContext context, TimeProvider timeProvider, ILogger logger)
    : ITransactionService
{
    public async Task<Result<TransactionResponse>> ProcessDepositAsync(Account account, decimal amount,
        CancellationToken cancellationToken)
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var deposit = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Type = TransactionType.Deposit,
                Amount = amount,
                TargetAccountNumber = account.AccountNumber,
                CreatedAt = timeProvider.GetUtcNow()
            };

            context.Transactions.Add(deposit);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.Information("Deposit successful: Account {AccountId}, Amount {Amount}", account.Id, amount);

            return Result.Success(new TransactionResponse(
                deposit.Id,
                deposit.AccountId,
                deposit.Type.GetDisplayName(),
                deposit.Amount,
                deposit.TargetAccountNumber,
                deposit.CreatedAt));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            logger.Error(ex,
                "Failed to process deposit for account {AccountId}: {Message}",
                account.Id,
                ex.Message);

            return Result.Failure<TransactionResponse>(TransactionError.Failed(ex.Message));
        }
    }
}
