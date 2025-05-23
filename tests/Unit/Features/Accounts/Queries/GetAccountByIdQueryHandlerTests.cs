using Application.Dtos.Account;
using Application.Features.Accounts.GetById;
using Domain.Accounts;
using FluentAssertions;
using Shared;

namespace Unit.Features.Accounts.Queries;

public sealed class GetAccountByIdQueryHandlerTests : AccountBaseTest
{
    [Fact]
    public async Task Handle_ShouldReturnAccount_WhenAccountExists()
    {
        // Arrange
        Accounts.Add(new Account
        {
            Id = FixedAccountId,
            OwnerName = "Smith",
            AccountType = AccountType.Business,
            Balance = 5000,
            AccountNumber = FixedAccountNumber,
            CreatedAt = FixedDate
        });

        var query = new GetAccountByIdQuery(FixedAccountId);
        var getAccountByIdQueryHandler = new GetAccountByIdQueryHandler(MockDbContext.Object);

        // Act
        Result<AccountResponse> result = await getAccountByIdQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(FixedAccountId);
        result.Value.OwnerName.Should().Be("Smith");
        result.Value.AccountNumber.Should().Be(FixedAccountNumber);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountDoesNotExist()
    {
        // Arrange
        var query = new GetAccountByIdQuery(FixedAccountId);

        var getAccountByIdQueryHandler = new GetAccountByIdQueryHandler(MockDbContext.Object);

        // Act
        Result<AccountResponse> result = await getAccountByIdQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NotFound($"{nameof(Account.Id)}.NotFound", $"Account with ID '{query.AccountId}' was not found."));
    }
}
