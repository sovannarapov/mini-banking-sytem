namespace Application.Dtos.Transaction;

public sealed record TransactionResponse(
    Guid Id,
    Guid AccountId,
    string Type,
    decimal Amount,
    string? TargetAccountNumber,
    DateTimeOffset Timestamp
);
