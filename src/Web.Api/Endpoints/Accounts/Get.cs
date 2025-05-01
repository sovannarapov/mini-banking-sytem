using Application.Dtos.Account;
using Application.Features.Accounts.Get;
using MediatR;
using Shared;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Accounts;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/accounts", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetAccountQuery();

            Result<List<AccountResponse>> result = await sender.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasApiVersion(1.0)
        .Produces<List<AccountResponse>>(StatusCodes.Status200OK)
        .WithSummary("Get all accounts")
        .WithDescription("Retrieves the details of all accounts.")
        .WithTags(Tags.Accounts);
    }
}
