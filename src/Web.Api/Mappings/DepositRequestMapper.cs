using Application.Common.Mappings;
using Application.Dtos.Transaction;
using Application.Features.Transactions.Deposit;

namespace Web.Api.Mappings;

internal sealed class DepositRequestMapper : IMapper<DepositTransactionRequest, DepositTransactionCommand>
{
    public DepositTransactionCommand Map(DepositTransactionRequest transactionRequest)
    {
        return new DepositTransactionCommand(transactionRequest.AccountId, transactionRequest.Amount);
    }
}
