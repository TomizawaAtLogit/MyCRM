using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspireApp1.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAssignmentToProjectsAndPreSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "customer_order_id",
                table: "projects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "customer_order_id",
                table: "presales_proposals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_customer_order_id",
                table: "projects",
                column: "customer_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_presales_proposals_customer_order_id",
                table: "presales_proposals",
                column: "customer_order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_presales_proposals_customer_orders_customer_order_id",
                table: "presales_proposals",
                column: "customer_order_id",
                principalTable: "customer_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_customer_orders_customer_order_id",
                table: "projects",
                column: "customer_order_id",
                principalTable: "customer_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_presales_proposals_customer_orders_customer_order_id",
                table: "presales_proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_customer_orders_customer_order_id",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "IX_projects_customer_order_id",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "IX_presales_proposals_customer_order_id",
                table: "presales_proposals");

            migrationBuilder.DropColumn(
                name: "customer_order_id",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "customer_order_id",
                table: "presales_proposals");
        }
    }
}
