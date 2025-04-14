using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Transactions;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(trans => trans.Id);

        builder.Property(trans => trans.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasComment("The type of transaction: Deposit, Withdrawal, and Transfer.")
            .IsRequired();

        builder.Property(trans => trans.Amount)
            .HasPrecision(18, 2);

        builder.HasIndex(trans => trans.AccountId);
        builder.HasIndex(trans => trans.Timestamp);
    }
}
