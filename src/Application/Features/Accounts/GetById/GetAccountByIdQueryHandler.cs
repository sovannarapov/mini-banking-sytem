using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Dtos.Account;
using Domain.Accounts;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Application.Features.Accounts.GetById;

internal sealed class GetAccountByIdQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetAccountByIdQuery, AccountResponse>
{
    public async Task<Result<AccountResponse>> Handle(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        AccountResponse? account = await context.Accounts
            .AsNoTracking()
            .Where(acc => acc.Id == query.AccountId)
            .Select(acc => new AccountResponse(acc.Id, acc.OwnerName, acc.AccountType.GetDisplayName(), 
                acc.Balance, acc.AccountNumber, acc.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);

        return account ?? Result.Failure<AccountResponse>(AccountError.NotFound(query.AccountId));
    }
}
