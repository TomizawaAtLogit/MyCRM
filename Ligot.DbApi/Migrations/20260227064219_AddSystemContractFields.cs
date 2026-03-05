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
            // Use IF NOT EXISTS to handle databases where these columns were already added
            // (e.g., via EnsureCreated or manual SQL), preventing duplicate-column errors.
            migrationBuilder.Sql(@"
                ALTER TABLE systems ADD COLUMN IF NOT EXISTS automatic_renewal boolean NULL;
                ALTER TABLE systems ADD COLUMN IF NOT EXISTS contract_date timestamp with time zone NULL;
                ALTER TABLE systems ADD COLUMN IF NOT EXISTS expiration_date timestamp with time zone NULL;
                ALTER TABLE systems ADD COLUMN IF NOT EXISTS health_check character varying(50) NULL;
                ALTER TABLE systems ADD COLUMN IF NOT EXISTS onsite_hours character varying(50) NULL;
                ALTER TABLE systems ADD COLUMN IF NOT EXISTS power_maintenance character varying(50) NULL;
                ALTER TABLE systems ADD COLUMN IF NOT EXISTS reception_hours character varying(50) NULL;
            ");
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
