using Application.Abstractions.Messaging;
using Domain.Transactions;

namespace Application.Features.Transactions.History;

public sealed record GetTransactionHistoryQuery : IQuery<TransactionHistoryResponse>
{
    public Guid AccountId { get; init; }
    public TransactionType? Type { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
