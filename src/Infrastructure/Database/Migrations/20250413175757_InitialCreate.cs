using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Accounts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AccountNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                AccountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The type of account: Savings, Checking, and Business."),
                Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Accounts", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Transactions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The type of transaction: Deposit, Withdrawal, and Transfer."),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TargetAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Transactions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Transactions_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_AccountNumber",
            table: "Accounts",
            column: "AccountNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_AccountId",
            table: "Transactions",
            column: "AccountId");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_CreatedAt",
            table: "Transactions",
            column: "CreatedAt");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Transactions");

        migrationBuilder.DropTable(
            name: "Accounts");
    }
}
