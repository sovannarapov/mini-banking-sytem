using Application.Common.Interfaces;
using Application.Dtos.Transaction;
using Domain.Constants;
using Domain.Transactions;
using Serilog;
using Shared;

namespace Infrastructure.Services;

public class ValidationService(ILogger logger) : IValidationService
{
    public Result ValidateDepositAmount(decimal amount)
    {
        switch (amount)
        {
            case <= TransactionConstants.MinTransactionAmount:
                logger.Warning("Invalid deposit amount: {Amount}", amount);
                return Result.Failure(TransactionError.InvalidAmount(amount));
            case > TransactionConstants.MaxTransactionAmount:
                logger.Warning("Deposit amount exceeds maximum limit: {Amount}", amount);
                return Result.Failure(TransactionError.ExceedsMaximumLimit(amount));
            default:
                return Result.Success();
        }
    }

    public Result ValidateWithdrawAmount(decimal currentBalance, decimal amount)
    {
        if (amount <= 0)
        {
            return Result.Failure(TransactionError.InvalidAmount(amount));
        }

        if (currentBalance < amount)
        {
            return Result.Failure(TransactionError.InsufficientBalance(amount, currentBalance));
        }

        return Result.Success();
    }
}
