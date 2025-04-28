using Application.Features.Transactions.History;
using MediatR;
using Shared;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Transactions;

internal sealed class GetHistory : IEndpoint
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
            })
            .HasApiVersion(1.0)
            .Produces<TransactionHistoryResponse>(StatusCodes.Status200OK)
            .WithSummary("Get transaction history by account id")
            .WithDescription("Retrieves the transaction history of an account by account id.")
            .WithTags(Tags.Accounts);
    }
}
