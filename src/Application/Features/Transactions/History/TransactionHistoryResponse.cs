namespace Application.Features.Transactions.History;

public record TransactionHistoryResponse(
    int TotalCount,
    decimal TotalInflow,
    decimal TotalOutflow,
    List<TransactionHistoryItem> Transactions
);
