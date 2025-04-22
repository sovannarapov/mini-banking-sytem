using Application.Abstractions.Data;
using Application.Interfaces;
using Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Features.Accounts;

public abstract class AccountBaseTest
{
    protected readonly Mock<IApplicationDbContext> MockDbContext;
    protected readonly Mock<IAccountNumberGenerator> MockAccountNumberGenerator;
    protected readonly Mock<TimeProvider> MockTimeProvider;
    protected readonly Mock<IGuidGenerator> MockGuidGenerator;

    protected readonly Guid FixedAccountId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    protected readonly DateTimeOffset FixedDate = DateTimeOffset.UtcNow;
    protected const string FixedAccountNumber = "0123456789";

    protected AccountBaseTest()
    {
        IEnumerable<Account> accounts = [];
        DbSet<Account> accountsDbSet = accounts.AsQueryable().BuildMockDbSet();

        MockDbContext = new Mock<IApplicationDbContext>();
        MockDbContext.Setup(applicationDbContext => applicationDbContext.Accounts).Returns(accountsDbSet);

        MockTimeProvider = new Mock<TimeProvider>();
        MockTimeProvider.Setup(timeProvider => timeProvider.GetUtcNow()).Returns(FixedDate);

        MockAccountNumberGenerator = new Mock<IAccountNumberGenerator>();
        MockAccountNumberGenerator.Setup(accountNumberGenerator => accountNumberGenerator.Generate()).Returns(FixedAccountNumber);

        MockGuidGenerator = new Mock<IGuidGenerator>();
        MockGuidGenerator.Setup(guidGenerator => guidGenerator.NewGuid()).Returns(FixedAccountId);
    }
}
