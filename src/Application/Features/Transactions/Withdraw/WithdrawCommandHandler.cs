using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;
using Domain.Accounts;
using Domain.Extensions;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Application.Features.Transactions.Withdraw;

internal sealed class WithdrawCommandHandler(IApplicationDbContext context, TimeProvider timeProvider) : ICommandHandler<WithdrawCommand, TransactionResponse>
{
    public async Task<Result<TransactionResponse>> Handle(WithdrawCommand command, CancellationToken cancellationToken)
    {
        Account? account = await context.Accounts.FirstOrDefaultAsync(acc => acc.Id == command.AccountId, cancellationToken);

        if (account is null)
        {
            return Result.Failure<TransactionResponse>(AccountError.NotFound(command.AccountId));
        }
        
        var transaction = new Transaction
        {
            AccountId = command.AccountId,
            Type = TransactionType.Withdraw,
            Amount = command.Amount,
            TargetAccountNumber = account.AccountNumber,
            Timestamp = timeProvider.GetUtcNow(),
        };
        
        context.Transactions.Add(transaction);
        
        await context.SaveChangesAsync(cancellationToken);
        
        var response = new TransactionResponse(
            Id: transaction.Id,
            AccountId: transaction.AccountId,
            Type: transaction.Type.GetDisplayName(),
            Amount: transaction.Amount,
            TargetAccountNumber: transaction.TargetAccountNumber,
            Timestamp: transaction.Timestamp
        );

        return Result.Success(response);
    }
}

