using System.Text.Json.Serialization;
using Domain.Transactions;

namespace Domain.Accounts;

public sealed class Account
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public string AccountNumber { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType AccountType { get; set; }
    public double Balance { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = [];
}
