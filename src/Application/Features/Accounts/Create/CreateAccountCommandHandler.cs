using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Dtos.Account;
using Application.Services;
using Domain.Accounts;
using Domain.Extensions;
using Shared;

namespace Application.Features.Accounts.Create;

internal sealed class CreateAccountCommandHandler(IApplicationDbContext context, TimeProvider timeProvider) : ICommandHandler<CreateAccountCommand, AccountResponse>
{
    public async Task<Result<AccountResponse>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            OwnerName = command.OwnerName,
            AccountNumber = AccountNumberGenerator.Generate(),
            AccountType = command.AccountType,
            Balance = 0,
            CreatedAt = timeProvider.GetUtcNow()
        };

        context.Accounts.Add(account);

        await context.SaveChangesAsync(cancellationToken);

        var response = new AccountResponse(
            Id: account.Id,
            OwnerName: account.OwnerName,
            AccountType: account.AccountType.GetDisplayName(),
            Balance: account.Balance,
            AccountNumber: account.AccountNumber,
            CreatedAt: account.CreatedAt
        );

        return Result.Success(response);
    }
}
