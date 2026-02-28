using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ligot.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAdminToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "roles");
        }
    }
}
