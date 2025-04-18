using Application.Abstractions.Messaging;
using Application.Dtos.Transaction;

namespace Application.Features.Transactions.Transfer;

public sealed record TransferTransactionCommand(Guid AccountId, string? TargetAccountNumber, decimal Amount) : ICommand<TransferResponse>;
