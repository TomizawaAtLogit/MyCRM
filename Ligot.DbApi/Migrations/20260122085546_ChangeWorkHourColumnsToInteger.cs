using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ligot.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWorkHourColumnsToInteger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // convert previously-decimal columns to integer
            migrationBuilder.AlterColumn<int>(
                name: "working_hours",
                table: "presales_work_hours",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "hourly_wage",
                table: "presales_work_hours",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "working_hours",
                table: "presales_work_hours",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "hourly_wage",
                table: "presales_work_hours",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
