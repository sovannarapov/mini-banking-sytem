namespace Application.Dtos.Transaction;

public sealed class WithdrawRequest
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}
