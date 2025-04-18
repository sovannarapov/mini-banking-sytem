using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;

namespace Application.Features.Transactions.Withdraw;

public sealed record WithdrawTransactionCommand(Guid AccountId, decimal Amount) : ICommand<TransactionResponse>;
