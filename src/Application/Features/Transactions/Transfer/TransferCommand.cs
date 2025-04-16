using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;

namespace Application.Features.Transactions.Transfer;

public sealed class TransferCommand : ICommand<TransactionResponse>
{
    public Guid AccountId { get; set; }
    public string? TargetAccountNumber { get; set; }
    public decimal Amount { get; set; }
}
