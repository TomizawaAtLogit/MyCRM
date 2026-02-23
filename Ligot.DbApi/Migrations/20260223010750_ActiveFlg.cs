using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ligot.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class ActiveFlg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "systems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "customer_systems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "customer_sites",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "customer_orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "case_relationships",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active_flg",
                table: "case_activities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "users");

            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "customer_systems");

            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "customer_sites");

            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "customer_orders");

            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "case_relationships");

            migrationBuilder.DropColumn(
                name: "active_flg",
                table: "case_activities");
        }
    }
}
