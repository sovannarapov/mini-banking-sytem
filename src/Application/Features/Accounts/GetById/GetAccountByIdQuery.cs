using Application.Abstractions.Messaging;
using Application.Dtos.Account;

namespace Application.Features.Accounts.GetById;

public sealed record GetAccountByIdQuery(Guid AccountId) : IQuery<AccountResponse>;
