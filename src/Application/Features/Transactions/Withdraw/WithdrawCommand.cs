using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;

namespace Application.Features.Transactions.Withdraw;

public sealed class WithdrawCommand : ICommand<TransactionResponse>
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}
