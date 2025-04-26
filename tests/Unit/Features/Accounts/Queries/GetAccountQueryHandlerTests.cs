using Application.Dtos.Account;
using Application.Features.Accounts.Get;
using Domain.Accounts;
using FluentAssertions;
using Shared;

namespace Unit.Features.Accounts.Queries;

public class GetAccountQueryHandlerTests : AccountBaseTest
{
    [Fact]
    public async Task Handle_ShouldReturnAccount_WhenAccountExists()
    {
        // Arrange
        Accounts.Add(new Account
        {
            Id = Guid.NewGuid(),
            OwnerName = "Alice",
            AccountType = AccountType.Checking,
            Balance = 500,
            AccountNumber = "1234567890",
            CreatedAt = FixedDate
        });

        Accounts.Add(new Account
        {
            Id = Guid.NewGuid(),
            OwnerName = "Bob",
            AccountType = AccountType.Savings,
            Balance = 1000,
            AccountNumber = "0987654321",
            CreatedAt = FixedDate
        });

        var query = new GetAccountQuery();

        var getAccountQueryHandler = new GetAccountQueryHandler(MockDbContext.Object);

        // Act
        Result<List<AccountResponse>> result = await getAccountQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenAccountDoesNotExist()
    {
        // Arrange
        var query = new GetAccountQuery();

        var getAccountQueryHandler = new GetAccountQueryHandler(MockDbContext.Object);

        // Act
        Result<List<AccountResponse>> result = await getAccountQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
