using Application.Common.Interfaces;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Tests.Features.Accounts;

namespace Tests.Features.Transactions;

public abstract class TransactionBaseTest : AccountBaseTest
{
    protected const decimal FixedAmount = 1000;
    protected const decimal InvalidFixedAmount = -1000m;
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
        MockAccountService = new Mock<IAccountService>();
        MockTransactionService = new Mock<ITransactionService>();
    }
}
