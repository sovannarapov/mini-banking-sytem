using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;

namespace Application.Features.Transactions.Deposit;

public sealed class DepositCommand : ICommand<TransactionResponse>
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}
