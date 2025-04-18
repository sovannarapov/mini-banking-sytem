using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;

namespace Application.Features.Transactions.Withdraw;

public sealed class WithdrawTransactionCommand : ICommand<TransactionResponse>
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}
