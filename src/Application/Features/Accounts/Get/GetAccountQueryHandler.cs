using System.Reflection.Metadata;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Dtos.Account;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Application.Features.Accounts.Get;

internal sealed class GetAccountQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAccountQuery, List<AccountResponse>>
{
    public async Task<Result<List<AccountResponse>>> Handle(GetAccountQuery query, CancellationToken cancellationToken)
    {
        List<AccountResponse> accounts = await context.Accounts
            .AsNoTracking()
            .Select(acc => new AccountResponse(
                acc.Id,
                acc.OwnerName,
                acc.AccountType.GetDisplayName(),
                acc.Balance, acc.AccountNumber,
                acc.CreatedAt)
            ).ToListAsync(cancellationToken);

        return accounts;
    }
}