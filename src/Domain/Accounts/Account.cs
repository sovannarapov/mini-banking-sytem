using System.ComponentModel;
using Domain.Transactions;

namespace Domain.Accounts;

public sealed class Account
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public string AccountNumber { get; set; }
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
}
