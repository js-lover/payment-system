using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace payment_system.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCardExpirationDateToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Card_CardNumber_Format",
                table: "Cards");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Card_CVC_Format",
                table: "Cards");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Card_ExpirationDate_Format",
                table: "Cards");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Card_CardNumber_Format",
                table: "Cards",
                sql: "length(CardNumber) = 16");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Card_CVC_Format",
                table: "Cards",
                sql: "length(CVC) = 3");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Card_ExpirationDate_Format",
                table: "Cards",
                sql: "length(ExpirationDate) = 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Card_CardNumber_Format",
                table: "Cards");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Card_CVC_Format",
                table: "Cards");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Card_ExpirationDate_Format",
                table: "Cards");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Card_CardNumber_Format",
                table: "Cards",
                sql: "length([CardNumber]) = 16 AND [CardNumber] NOT GLOB '*[^0-9]*'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Card_CVC_Format",
                table: "Cards",
                sql: "length([CVC]) = 3 AND [CVC] NOT GLOB '*[^0-9]*'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Card_ExpirationDate_Format",
                table: "Cards",
                sql: "length([ExpirationDate]) = 5 AND [ExpirationDate] LIKE '__/__' AND [ExpirationDate] NOT LIKE '%[^0-9/]%'");
        }
    }
}
