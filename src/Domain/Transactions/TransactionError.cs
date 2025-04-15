using Shared;

namespace Domain.Transactions;

public static class TransactionError
{
    public static Error ExceedsMaximumLimit(decimal amount) => Error.Failure(
        $"{nameof(Transaction)}.ExceedsMaximumLimit",
        $"The amount {amount} exceeds the maximum deposit limit.");

    public static Error Failed(string message) => Error.Failure(
        "Transaction.Failed",
        $"Transaction failed: {message}");

    public static Error InsufficientFunds(decimal balance, decimal amount) => Error.Failure(
        $"{nameof(Transaction)}.InsufficientFunds",
        $"Account balance {balance} is insufficient for withdrawal amount {amount}.");

    public static Error InvalidAmount(decimal amount) => Error.Failure(
        "Transaction.InvalidAmount",
        $"{amount} must be greater than zero."
    );
}
