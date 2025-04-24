using Application.Common.Interfaces;
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
            case > TransactionConstants.MaxDepositAmount:
                logger.Warning("Deposit amount exceeds maximum limit: {Amount}", amount);
                return Result.Failure(TransactionError.ExceedsMaximumLimit(amount));
            default:
                return Result.Success();
        }
    }
}
