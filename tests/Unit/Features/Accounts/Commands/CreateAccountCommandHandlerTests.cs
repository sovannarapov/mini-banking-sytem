using Application.Dtos.Account;
using Application.Features.Accounts.Create;
using Domain.Accounts;
using Domain.Constants;
using Domain.Extensions;
using Shared;

namespace Unit.Features.Accounts.Commands;

public class CreateAccountCommandHandlerTests : AccountBaseTest
{
    private readonly CreateAccountCommandHandler _handler;

    public CreateAccountCommandHandlerTests()
    {
        _handler = new CreateAccountCommandHandler(
            MockDbContext.Object,
            MockAccountNumberGenerator.Object,
            MockTimeProvider.Object,
            MockGuidGenerator.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateAccount_WhenAllRequiredFieldsProvided()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            OwnerName = "John Doe",
            AccountType = AccountType.Savings
        };

        // Act
        Result<AccountResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.OwnerName.Should().Be("John Doe");
        result.Value.AccountNumber.Should().Be(FixedAccountNumber);
        result.Value.Id.Should().Be(FixedAccountId);
        result.Value.Should().BeEquivalentTo(new AccountResponse
        (
            FixedAccountId,
            command.OwnerName,
            command.AccountType.GetDisplayName(),
            0,
            result.Value.AccountNumber,
            FixedDate
        ));

        MockDbContext.Verify(applicationDbContext => applicationDbContext.Accounts.Add(
                It.Is<Account>(account =>
                    account.OwnerName == command.OwnerName &&
                    account.Balance == AccountConstants.MinBalance &&
                    account.AccountType == command.AccountType)),
            Times.Once);
        MockDbContext.Verify(applicationDbContext => applicationDbContext.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotCreateAccount_WhenOwnerNameIsMissing()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            OwnerName = "",
            AccountType = AccountType.Savings
        };

        // Act
        Result<AccountResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Failure($"{nameof(command.OwnerName)}.Required", $"{nameof(command.OwnerName)} is required."));

        MockDbContext.Verify(applicationDbContext => applicationDbContext.Accounts.Add(
                It.Is<Account>(account =>
                    account.OwnerName == command.OwnerName &&
                    account.Balance == AccountConstants.MinBalance &&
                    account.AccountType == command.AccountType)),
            Times.Never);
        MockDbContext.Verify(applicationDbContext => applicationDbContext.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCreateAccount_WhenAccountTypeIsMissing()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            OwnerName = "John Doe",
            AccountType = default
        };

        // Act
        Result<AccountResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Failure($"{nameof(command.AccountType)}.Required", $"{nameof(command.AccountType)} is required."));

        MockDbContext.Verify(applicationDbContext => applicationDbContext.Accounts.Add(
                It.Is<Account>(account =>
                    account.OwnerName == command.OwnerName &&
                    account.Balance == AccountConstants.MinBalance &&
                    account.AccountType == command.AccountType)),
            Times.Never);
        MockDbContext.Verify(applicationDbContext => applicationDbContext.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
