namespace Application.Dtos.Transaction;

public sealed class TransferRequest
{
    public Guid AccountId { get; set; }
    public string TargetAccountNumber { get; set; }
    public decimal Amount { get; set; }
}
