using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace payment_system.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NationalId = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 11, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.CheckConstraint("CK_Customer_NationalId_Format", "length(NationalId) = 11 AND NationalId NOT GLOB '*[^0-9]*'");
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 26, nullable: false),
                    Balance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.CheckConstraint("CK_Account_AccountNumber_Format", "length([AccountNumber]) = 26 AND [AccountNumber] GLOB 'TR[0-9]*'");
                    table.ForeignKey(
                        name: "FK_Accounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CardNumber = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 16, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", fixedLength: true, maxLength: 5, nullable: false),
                    CVC = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 3, nullable: false),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.CheckConstraint("CK_Card_CardNumber_Format", "length([CardNumber]) = 16 AND [CardNumber] NOT GLOB '*[^0-9]*'");
                    table.CheckConstraint("CK_Card_CVC_Format", "length([CVC]) = 3 AND [CVC] NOT GLOB '*[^0-9]*'");
                    table.CheckConstraint("CK_Card_ExpirationDate_Format", "length([ExpirationDate]) = 5 AND [ExpirationDate] LIKE '__/__' AND [ExpirationDate] NOT LIKE '%[^0-9/]%'");
                    table.ForeignKey(
                        name: "FK_Cards_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CardId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TransactionType = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Currency = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ReferenceTransactionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
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
                    table.ForeignKey(
                        name: "FK_Transactions_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Transactions_ReferenceTransactionId",
                        column: x => x.ReferenceTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Email", "IsDeleted", "Name", "NationalId", "Surname", "UpdatedAt" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "floyd@example.com", false, "Floyd", "12345678901", "Pro", null });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountNumber", "Balance", "CreatedAt", "Currency", "CustomerId", "DeletedAt", "IsDeleted", "UpdatedAt" },
                values: new object[] { new Guid("b2c3d4e5-f6a7-4b5c-9d8e-1f2a3b4c5d6e"), "TR001234567890123456789012", 10000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TRY", new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), null, false, null });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CardId", "CreatedAt", "Currency", "DeletedAt", "Description", "ReferenceTransactionId", "Status", "TransactionDate", "TransactionType", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c3d4e5f6-a7b8-4c5d-8e9f-2a3b4c5d6e7f"), new Guid("b2c3d4e5-f6a7-4b5c-9d8e-1f2a3b4c5d6e"), 500.00m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, "Market Alışverişi", null, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null },
                    { new Guid("d4e5f6a7-b8c9-4d5e-9f0a-3b4c5d6e7f8a"), new Guid("b2c3d4e5-f6a7-4b5c-9d8e-1f2a3b4c5d6e"), 100.00m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, "Ürün İadesi", new Guid("c3d4e5f6-a7b8-4c5d-8e9f-2a3b4c5d6e7f"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CustomerId",
                table: "Accounts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AccountId",
                table: "Cards",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardNumber",
                table: "Cards",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NationalId",
                table: "Customers",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CardId",
                table: "Transactions",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReferenceTransactionId",
                table: "Transactions",
                column: "ReferenceTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
