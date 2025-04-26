using Application.Common.Mappings;
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
                async (DepositTransactionRequest request, ISender sender,
                    IMapper<DepositTransactionRequest, DepositTransactionCommand> mapper,
                    CancellationToken cancellationToken) =>
                {
                    DepositTransactionCommand command = mapper.Map(request);

                    Result<TransactionResponse> result = await sender.Send(command, cancellationToken);

                    return result.Match(Results.Ok, CustomResults.Problem);
                })
            .HasApiVersion(1.0)
            .Produces<TransactionResponse>()
            .WithSummary("Create a new deposit transaction")
            .WithDescription("Creates a new deposit transaction for the specified account.")
            .WithTags(Tags.Transactions);
    }
}
