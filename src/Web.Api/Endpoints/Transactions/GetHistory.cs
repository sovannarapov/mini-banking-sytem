using Application.Features.Transactions.History;
using MediatR;
using Shared;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Transactions;

internal sealed class TransactionHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/accounts/{accountId:guid}/transactions",
            async (Guid accountId, [AsParameters] GetTransactionHistoryQuery query, ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<TransactionHistoryResponse> result =
                    await sender.Send(query with { AccountId = accountId }, cancellationToken);

                return result.Match(Results.Ok<TransactionHistoryResponse>, CustomResults.Problem);
            });
    }
}
