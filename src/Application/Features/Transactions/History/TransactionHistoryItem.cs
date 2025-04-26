using Domain.Transactions;

namespace Application.Features.Transactions.History;

public record TransactionHistoryItem(
    Guid Id, 
    string Type, 
    decimal Amount, 
    string? TargetAccountNumber, 
    DateTimeOffset CreatedAt
);
