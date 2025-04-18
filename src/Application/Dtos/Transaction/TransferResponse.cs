namespace Application.Dtos.Transaction;

public sealed record TransferResponse(Guid CreditTransactionId, Guid DebitTransactionId);
