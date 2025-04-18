using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;

namespace Application.Features.Transactions.Deposit;

public sealed record DepositTransactionCommand(Guid AccountId, decimal Amount) : ICommand<TransactionResponse>;
