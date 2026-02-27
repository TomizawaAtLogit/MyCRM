using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ligot.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemContractFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "automatic_renewal",
                table: "systems",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "contract_date",
                table: "systems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expiration_date",
                table: "systems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "health_check",
                table: "systems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "onsite_hours",
                table: "systems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "power_maintenance",
                table: "systems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reception_hours",
                table: "systems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "automatic_renewal",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "contract_date",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "expiration_date",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "health_check",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "onsite_hours",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "power_maintenance",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "reception_hours",
                table: "systems");
        }
    }
}
