﻿// <auto-generated />
using System;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Database.Migrations;

[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.4")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("Domain.Accounts.Account", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("AccountNumber")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.Property<int>("AccountType")
                    .HasColumnType("int");

                b.Property<double>("Balance")
                    .HasColumnType("float");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("OwnerName")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.HasIndex("AccountNumber")
                    .IsUnique();

                b.ToTable("Accounts", (string)null);
            });

        modelBuilder.Entity("Domain.Transactions.Transaction", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("AccountId")
                    .HasColumnType("uniqueidentifier");

                b.Property<double>("Amount")
                    .HasColumnType("float");

                b.Property<string>("TargetAccountNumber")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("Timestamp")
                    .HasColumnType("datetime2");

                b.Property<int>("Type")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("AccountId");

                b.HasIndex("Timestamp");

                b.ToTable("Transactions", (string)null);
            });

        modelBuilder.Entity("Domain.Transactions.Transaction", b =>
            {
                b.HasOne("Domain.Accounts.Account", "Account")
                    .WithMany("Transactions")
                    .HasForeignKey("AccountId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Account");
            });

        modelBuilder.Entity("Domain.Accounts.Account", b =>
            {
                b.Navigation("Transactions");
            });
#pragma warning restore 612, 618
    }
}

