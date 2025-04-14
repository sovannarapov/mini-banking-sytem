namespace Application.Dtos.Account;

public sealed record AccountResponse(
    Guid Id,
    string OwnerName,
    string AccountType,
    decimal Balance,
    string AccountNumber,
    DateTimeOffset CreatedAt
);
