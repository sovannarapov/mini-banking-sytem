using Application.Dtos.Transaction;
using Application.Features.Transactions.Deposit;
using Domain.Accounts;
using Domain.Extensions;
using Domain.Transactions;
using FluentAssertions;
using Moq;
using Shared;

namespace Tests.Features.Transactions.Deposit;

public sealed class DepositTransactionCommandHandlerTests : TransactionBaseTest
{
    private readonly DepositTransactionCommandHandler _handler;

    public DepositTransactionCommandHandlerTests()
    {
        _handler = new DepositTransactionCommandHandler(
            MockValidationService.Object,
            MockAccountService.Object,
            MockTransactionService.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAmountIsInvalid()
    {
        // Arrange
        var command = new DepositTransactionCommand(FixedAccountId, InvalidFixedAmount);
        Error error = TransactionError.InvalidAmount(InvalidFixedAmount);

        MockValidationService.Setup(validationService => validationService.ValidateDepositAmount(It.IsAny<decimal>()))
            .Returns(Result.Failure(error));

        // Act
        Result<TransactionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccountNotFound()
    {
        // Arrange
        var command = new DepositTransactionCommand(FixedAccountId, FixedAmount);
        Error error = AccountError.NotFound(FixedAccountId);

        MockAccountService
            .Setup(accountService =>
                accountService.GetAccountByIdAsync(FixedAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act
        Result<TransactionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_ShouldCallUpdateBalance_AndProcessDeposit_WhenCommandIsValid()
    {
        // Arrange
        var command = new DepositTransactionCommand(FixedAccountId, FixedAmount);
        Account account = new();

        MockAccountService
            .Setup(accountService =>
                accountService.GetAccountByIdAsync(FixedAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var expectedResponse = new TransactionResponse(Guid.NewGuid(), FixedAccountId, TransactionType.Deposit.GetDisplayName(), FixedAmount, FixedAccountNumber, FixedDate);

        MockTransactionService.Setup(s => s.ProcessDepositAsync(account, FixedAmount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        Result<TransactionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResponse);

        MockAccountService.Verify(s => s.UpdateBalance(account, FixedAmount), Times.Once);
        MockTransactionService.Verify(s => s.ProcessDepositAsync(account, FixedAmount, It.IsAny<CancellationToken>()), Times.Once);
    }
}
