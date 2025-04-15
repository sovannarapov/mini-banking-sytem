using Application.Dtos.Account;
using Application.Dtos.Transaction;
using Application.Features.Transactions.Withdraw;
using MediatR;
using Shared;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Transactions;

internal sealed class Withdraw : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/transactions/withdraw", async (WithdrawRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new WithdrawCommand
            {
                AccountId = request.AccountId,
                Amount = request.Amount
            };
            
            Result<TransactionResponse> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasApiVersion(1.0)
        .Produces<AccountResponse>(StatusCodes.Status200OK)
        .WithSummary("Create a new withdraw transaction")
        .WithDescription("Creates a new withdraw transaction for the specified account.")
        .WithTags(Tags.Transactions);
    }
}

