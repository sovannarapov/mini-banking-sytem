using Domain.Accounts;

namespace Domain.Transactions;

public sealed class Transaction
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string? TargetAccountNumber { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Account Account { get; set; }
}
