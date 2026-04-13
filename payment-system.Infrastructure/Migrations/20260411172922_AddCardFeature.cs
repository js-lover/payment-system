using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace payment_system.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCardFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardName",
                table: "Cards",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardName",
                table: "Cards",
                column: "CardName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cards_CardName",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CardName",
                table: "Cards");
        }
    }
}
