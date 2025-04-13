using Application.Dtos.Account;
using Application.Features.Accounts.GetById;
using MediatR;
using Shared;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Accounts;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/accounts/{accountId:guid}", async (Guid accountId, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetAccountByIdQuery(accountId);

            Result<AccountResponse> result = await sender.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasApiVersion(1.0)
        .Produces<AccountResponse>(StatusCodes.Status200OK)
        .WithSummary("Get an account by ID")
        .WithDescription("Retrieves the details of an account by its unique identifier.")
        .WithTags(Tags.Accounts);
    }
}