using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace payment_system.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CleanComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Card_CVC_Format",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CVC",
                table: "Cards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CVC",
                table: "Cards",
                type: "TEXT",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Card_CVC_Format",
                table: "Cards",
                sql: "length(CVC) = 3");
        }
    }
}
