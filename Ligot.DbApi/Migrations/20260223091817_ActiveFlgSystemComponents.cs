using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ligot.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class ActiveFlgSystemComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "system_components",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "system_components");
        }
    }
}
