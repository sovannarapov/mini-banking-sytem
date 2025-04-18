namespace Application.Dtos.Transaction;

public sealed record TransferRequest(Guid AccountId, string TargetAccountNumber, decimal Amount);
