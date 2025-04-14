namespace Application.Dtos.Transaction;

public sealed class DepositRequest
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}
