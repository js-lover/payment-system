using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace payment_system.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Email", "IsDeleted", "Name", "NationalId", "Surname", "UpdatedAt" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), new DateTime(2026, 3, 4, 18, 37, 1, 841, DateTimeKind.Utc).AddTicks(1450), null, "floyd@example.com", false, "Floyd", "12345678901", "Pro", null });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountNumber", "Balance", "CreatedAt", "Currency", "CustomerId", "DeletedAt", "IsDeleted", "UpdatedAt" },
                values: new object[] { new Guid("b2c3d4e5-f6a7-4b5c-9d8e-1f2a3b4c5d6e"), "TR1234567890", 10000m, new DateTime(2026, 3, 4, 18, 37, 1, 841, DateTimeKind.Utc).AddTicks(4150), "TRY", new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), null, false, null });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "CardId", "CreatedAt", "DeletedAt", "Description", "ReferenceTransactionId", "Status", "TransactionDate", "TransactionType", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c3d4e5f6-a7b8-4c5d-8e9f-2a3b4c5d6e7f"), new Guid("b2c3d4e5-f6a7-4b5c-9d8e-1f2a3b4c5d6e"), 500.00m, null, new DateTime(2026, 3, 4, 18, 37, 1, 841, DateTimeKind.Utc).AddTicks(4960), null, "Market Alışverişi", null, 1, new DateTime(2026, 3, 4, 16, 37, 1, 841, DateTimeKind.Utc).AddTicks(4680), 1, null },
                    { new Guid("d4e5f6a7-b8c9-4d5e-9f0a-3b4c5d6e7f8a"), new Guid("b2c3d4e5-f6a7-4b5c-9d8e-1f2a3b4c5d6e"), 100.00m, null, new DateTime(2026, 3, 4, 18, 37, 1, 841, DateTimeKind.Utc).AddTicks(5100), null, "Ürün İadesi", new Guid("c3d4e5f6-a7b8-4c5d-8e9f-2a3b4c5d6e7f"), 1, new DateTime(2026, 3, 4, 17, 37, 1, 841, DateTimeKind.Utc).AddTicks(5000), 2, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-4d5e-9f0a-3b4c5d6e7f8a"));

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-4c5d-8e9f-2a3b4c5d6e7f"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-4b5c-9d8e-1f2a3b4c5d6e"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"));
        }
    }
}
