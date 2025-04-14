using Application.Dtos.Transaction;
using Application.Features.Transactions.Deposit;
using MediatR;
using Shared;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Transactions;

internal sealed class Deposit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/transactions/deposit",
            async (DepositRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new DepositCommand
                {
                    AccountId = request.AccountId,
                    Amount = request.Amount
                };

                Result<TransactionResponse> result = await sender.Send(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            });
    }
}
