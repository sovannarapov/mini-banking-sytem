using Application.Dtos.Transaction;
using Application.Features.Transactions.Deposit;
using Domain.Transactions;
using FluentAssertions;
using Shared;

namespace Tests.Features.Transactions.Deposit;

public sealed class DepositTransactionCommandHandlerTests : TransactionBaseTest
{
    private readonly DepositTransactionCommandHandler _handler;

    public DepositTransactionCommandHandlerTests() => _handler = new DepositTransactionCommandHandler(
            MockValidationService.Object,
            MockAccountService.Object,
            MockTransactionService.Object
        );

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAmountIsInvalid()
    {
        // Arrange
        var command = new DepositTransactionCommand(FixedAccountId, FixedAmount);
        Error error = TransactionError.InvalidAmount(FixedAmount);

        MockValidationService.Setup(v => v.ValidateDepositAmount(FixedAmount))
            .Returns(Result.Failure(error));

        // Act
        Result<TransactionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
