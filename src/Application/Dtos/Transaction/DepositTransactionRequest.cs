namespace Application.Dtos.Transaction;

public sealed class DepositTransactionRequest
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}
