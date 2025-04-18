using Application.Dtos.Transaction;
using Application.Features.Transactions.Transfer;
using MediatR;
using Shared;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Transactions;

internal sealed class Transfer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/transactions/transfer",
            async (TransferRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new TransferTransactionCommand(
                    request.AccountId,
                    request.TargetAccountNumber,
                    request.Amount
                );

                Result<TransferResponse> result = await sender.Send(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .HasApiVersion(1.0)
            .Produces<TransactionResponse>(StatusCodes.Status200OK)
            .WithSummary("Create a new transfer transaction")
            .WithDescription("Creates a new transfer transaction for the specified account.")
            .WithTags(Tags.Transactions);
    }
}
