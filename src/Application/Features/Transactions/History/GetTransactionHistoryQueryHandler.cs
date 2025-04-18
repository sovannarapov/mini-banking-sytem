using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Application.Features.Transactions.History;

internal sealed class GetTransactionHistoryQueryHandler(IApplicationDbContext context) : IQueryHandler<GetTransactionHistoryQuery, TransactionHistoryResponse>
{
    public async Task<Result<TransactionHistoryResponse>> Handle(GetTransactionHistoryQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Transaction> query = context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == request.AccountId);

        if (request.Type is not null)
        {
            query = query.Where(t => t.Type == request.Type);
        }

        if (request.FromDate is not null)
        {
            query = query.Where(t => t.CreatedAt >= request.FromDate);
        }

        if (request.ToDate is not null)
        {
            query = query.Where(t => t.CreatedAt <= request.ToDate);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        decimal inflow = await query
            .Where(t => t.Amount > 0)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0;

        decimal outflow = await query
            .Where(t => t.Amount < 0)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0;

        List<TransactionHistoryItem> items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TransactionHistoryItem(
                t.Id,
                t.Type,
                t.Amount,
                t.TargetAccountNumber,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new TransactionHistoryResponse(
            totalCount,
            inflow,
            outflow,
            items
        );
    }
}
