using Application.Common.Interfaces;
using Application.Dtos.Transaction;
using Domain.Accounts;
using Domain.Extensions;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Shared;
using Tests.Features.Accounts;

namespace Tests.Features.Transactions;

public abstract class TransactionBaseTest : AccountBaseTest
{
    protected const decimal FixedAmount = 1000;
    protected const decimal InvalidFixedAmount = -1000m;
    protected readonly Guid FixedTransactionId = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff");
    protected readonly Mock<IAccountService> MockAccountService;
    protected readonly Mock<ITransactionService> MockTransactionService;
    protected readonly Mock<IValidationService> MockValidationService;

    protected readonly List<Transaction> Transactions = [];

    protected TransactionBaseTest()
    {
        Mock<DbSet<Transaction>> transactionsDbSet = Transactions.AsQueryable().BuildMockDbSet();

        MockDbContext.Setup(applicationDbContext => applicationDbContext.Transactions)
            .Returns(transactionsDbSet.Object);
        MockTimeProvider.Setup(timeProvider => timeProvider.GetUtcNow()).Returns(FixedDate);
        MockAccountNumberGenerator.Setup(accountNumberGenerator => accountNumberGenerator.Generate())
            .Returns(FixedAccountNumber);
        MockGuidGenerator.Setup(guidGenerator => guidGenerator.NewGuid()).Returns(FixedAccountId);

        MockValidationService = new Mock<IValidationService>();
        MockValidationService.Setup(validationService => validationService.ValidateDepositAmount(It.IsAny<decimal>()))
            .Returns(Result.Success);

        MockAccountService = new Mock<IAccountService>();
        MockAccountService
            .Setup(accountService =>
                accountService.GetAccountByIdAsync(FixedAccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Account());
        MockAccountService
            .Setup(accountService => accountService.UpdateBalance(It.IsAny<Account>(), It.IsAny<decimal>()))
            .Callback<Account, decimal>((acc, amount) => acc.Balance += amount);

        MockTransactionService = new Mock<ITransactionService>();
        MockTransactionService
            .Setup(t => t.ProcessDepositAsync(It.IsAny<Account>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionResponse(
                FixedTransactionId,
                FixedAccountId,
                TransactionType.Deposit.GetDisplayName(),
                FixedAmount,
                FixedAccountNumber,
                FixedDate
            ));
    }
}
