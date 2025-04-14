using Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Accounts;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(acc => acc.Id);

        builder
            .HasIndex(acc => acc.AccountNumber)
            .IsUnique();

        builder.Property(acc => acc.AccountType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasComment("The type of account: Savings, Checking, and Business.")
            .IsRequired();

        builder.Property(acc => acc.Balance)
            .HasPrecision(18, 2);
    }
}
