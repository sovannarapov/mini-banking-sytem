using Shared;

namespace Application.Common.Interfaces;

public interface IValidationService
{
    Result ValidateDepositAmount(decimal amount);
    Result ValidateWithdrawAmount(decimal currentBalance, decimal amount);
}
