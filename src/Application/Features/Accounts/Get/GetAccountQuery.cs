using Application.Abstractions.Messaging;
using Application.Dtos.Account;

namespace Application.Features.Accounts.Get;

public sealed record GetAccountQuery : IQuery<List<AccountResponse>>;
