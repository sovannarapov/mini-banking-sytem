using Domain.Transactions;

namespace Application.Features.Transactions.History;

public record TransactionHistoryItem(
    Guid Id, 
    TransactionType Type, 
    decimal Amount, 
    string? TargetAccountNumber, 
    DateTimeOffset CreatedAt
);
