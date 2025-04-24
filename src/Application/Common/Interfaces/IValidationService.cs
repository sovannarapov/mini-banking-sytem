using Shared;

namespace Application.Common.Interfaces;

public interface IValidationService
{
    Result ValidateDepositAmount(decimal amount);
}
