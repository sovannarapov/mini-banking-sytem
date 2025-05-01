namespace Application.Dtos.Transaction;

public record TransactionHistoryItem(
    Guid Id,
    string Type,
    decimal Amount,
    string? TargetAccountNumber,
    DateTimeOffset CreatedAt
);
