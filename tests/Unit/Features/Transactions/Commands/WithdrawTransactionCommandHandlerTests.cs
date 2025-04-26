using Application.Dtos.Transaction;
using Application.Features.Transactions.Withdraw;
using Domain.Accounts;
using Domain.Extensions;
using Domain.Transactions;
using FluentAssertions;
using Moq;
using Shared;

namespace Unit.Features.Transactions.Commands;

public class WithdrawTransactionCommandHandlerTests : TransactionBaseTest
{
    private readonly WithdrawTransactionCommandHandler _handler;

    public WithdrawTransactionCommandHandlerTests()
    {
        _handler = new WithdrawTransactionCommandHandler(
            MockValidationService.Object,
            MockAccountService.Object,
            MockTransactionService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
    {
        // Arrange
        var command = new WithdrawTransactionCommand(FixedAccountId, InvalidFixedAmount);

        MockAccountService
            .Setup(accountService => accountService.GetAccountByIdAsync(FixedAccountId, CancellationToken.None))
            .ReturnsAsync((Account?)null);

        // Act
        Result<TransactionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AccountError.NotFound(FixedAccountId));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFailed()
    {
        // Arrange
        var command = new WithdrawTransactionCommand(FixedAccountId, InvalidFixedAmount);
        Error error = TransactionError.InvalidAmount(InvalidFixedAmount);
        Account account = new();

        MockAccountService
            .Setup(accountService => accountService.GetAccountByIdAsync(FixedAccountId, CancellationToken.None))
            .ReturnsAsync(account);

        MockValidationService
            .Setup(validationService => validationService.ValidateWithdrawAmount(account.Balance, InvalidFixedAmount))
            .Returns(Result.Failure(error));

        // Act
        Result<TransactionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenWithdrawIsValid()
    {
        // Arrange
        var command = new WithdrawTransactionCommand(FixedAccountId, FixedAmount);
        var expectedResponse = new TransactionResponse(Guid.NewGuid(), FixedAccountId,
            TransactionType.Withdraw.GetDisplayName(), FixedAmount, FixedAccountNumber, FixedDate);
        Account account = new();

        MockAccountService
            .Setup(accountService => accountService.GetAccountByIdAsync(FixedAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        MockValidationService
            .Setup(validationService => validationService
                .ValidateWithdrawAmount(It.IsAny<decimal>(),It.IsAny<decimal>()))
            .Returns(Result.Success);

        MockTransactionService
            .Setup(transactionService => transactionService
                .ProcessWithdrawalAsync(account, FixedAmount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedResponse));

        MockAccountService.Setup(accountService => accountService
            .Withdraw(account, FixedAmount)).Returns(Result.Success);

        // Act
        Result<TransactionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResponse);

        MockAccountService.Verify(accountService => accountService.Withdraw(account, FixedAmount),
            Times.Once);
        MockTransactionService.Verify(transactionService => transactionService
            .ProcessWithdrawalAsync(account, FixedAmount, It.IsAny<CancellationToken>()));
    }
}
