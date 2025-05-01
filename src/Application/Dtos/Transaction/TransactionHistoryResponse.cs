namespace Application.Dtos.Transaction;

public record TransactionHistoryResponse(
    int TotalCount,
    decimal TotalInflow,
    decimal TotalOutflow,
    List<TransactionHistoryItem> Transactions
);
