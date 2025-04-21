using Application.Abstractions.Data;
using Application.Dtos.Account;
using Application.Features.Accounts.Create;
using Application.Interfaces;
using Domain.Accounts;
using Domain.Constants;
using Domain.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared;

namespace Tests.Features.Accounts.Create;

public class CreateAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateAccount_WhenCommandIsValid()
    {
        // Arrange
        IEnumerable<Account> accounts = [];
        DbSet<Account> accountsDbSet = accounts.AsQueryable().BuildMockDbSet();
        var mockDbContext = new Mock<IApplicationDbContext>();
        mockDbContext.Setup(applicationDbContext => applicationDbContext.Accounts).Returns(accountsDbSet);

        var fixedDate = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var mockTimeProvider = new Mock<TimeProvider>();
        mockTimeProvider.Setup(timeProvider => timeProvider.GetUtcNow()).Returns(fixedDate);

        const string fixedAccountNumber = "0123456789";
        var mockAccountNumberGenerator = new Mock<IAccountNumberGenerator>();
        mockAccountNumberGenerator.Setup(accountNumberGenerator => accountNumberGenerator.Generate())
            .Returns(fixedAccountNumber);

        var accountId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var mockGuidGenerator = new Mock<IGuidGenerator>();
        mockGuidGenerator.Setup(guidGenerator => guidGenerator.NewGuid()).Returns(accountId);

        var command = new CreateAccountCommand
        {
            OwnerName = "John Doe",
            AccountType = AccountType.Savings
        };

        var createAccountHandler = new CreateAccountCommandHandler(
            mockDbContext.Object,
            mockAccountNumberGenerator.Object,
            mockTimeProvider.Object,
            mockGuidGenerator.Object);

        // Act
        Result<AccountResponse> result = await createAccountHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new AccountResponse
        (
            accountId,
            command.OwnerName,
            command.AccountType.GetDisplayName(),
            0,
            result.Value.AccountNumber,
            fixedDate
        ));

        mockDbContext.Verify(applicationDbContext => applicationDbContext.Accounts.Add(
                It.Is<Account>(account =>
                    account.OwnerName == command.OwnerName &&
                    account.Balance == AccountConstants.MinBalance &&
                    account.AccountType == command.AccountType)),
            Times.Once);
        mockDbContext.Verify(applicationDbContext => applicationDbContext.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotCreateAccount_WhenCommandIsInvalid()
    {
        // Arrange
        IEnumerable<Account> accounts = [];
        DbSet<Account> accountsDbSet = accounts.AsQueryable().BuildMockDbSet();
        var mockDbContext = new Mock<IApplicationDbContext>();
        mockDbContext.Setup(applicationDbContext => applicationDbContext.Accounts).Returns(accountsDbSet);

        var fixedDate = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var mockTimeProvider = new Mock<TimeProvider>();
        mockTimeProvider.Setup(timeProvider => timeProvider.GetUtcNow()).Returns(fixedDate);

        const string fixedAccountNumber = "0123456789";
        var mockAccountNumberGenerator = new Mock<IAccountNumberGenerator>();
        mockAccountNumberGenerator.Setup(accountNumberGenerator => accountNumberGenerator.Generate())
            .Returns(fixedAccountNumber);

        var accountId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var mockGuidGenerator = new Mock<IGuidGenerator>();
        mockGuidGenerator.Setup(guidGenerator => guidGenerator.NewGuid()).Returns(accountId);

        var command = new CreateAccountCommand
        {
            OwnerName = "",
            AccountType = AccountType.Savings,
        };

        var createAccountHandler = new CreateAccountCommandHandler(
            mockDbContext.Object,
            mockAccountNumberGenerator.Object,
            mockTimeProvider.Object,
            mockGuidGenerator.Object);

        // Act
        Result<AccountResponse> result = await createAccountHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        
        mockDbContext.Verify(applicationDbContext => applicationDbContext.Accounts.Add(
                It.Is<Account>(account =>
                    account.OwnerName == command.OwnerName &&
                    account.Balance == AccountConstants.MinBalance &&
                    account.AccountType == command.AccountType)),
            Times.Never);
        mockDbContext.Verify(applicationDbContext => applicationDbContext.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
