using System.Text.Json.Serialization;
using Domain.Accounts;

namespace Domain.Transactions;

public sealed class Transaction
{
    public Guid Id { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionType Type { get; set; }
    public double Amount { get; set; }
    public string? TargetAccountNumber { get; set; }
    public DateTime Timestamp { get; set; }
    
    public Guid AccountId { get; set; }
    public Account Account { get; set; }
}
