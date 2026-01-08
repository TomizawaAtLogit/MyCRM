using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspireApp1.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingCaseColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "customer_order_id",
                table: "cases",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "customer_site_id",
                table: "cases",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "system_component_id",
                table: "cases",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "system_id",
                table: "cases",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "display_order",
                table: "case_templates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "case_relationships",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "case_relationships",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_cases_customer_order_id",
                table: "cases",
                column: "customer_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_customer_site_id",
                table: "cases",
                column: "customer_site_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_first_response_at",
                table: "cases",
                column: "first_response_at");

            migrationBuilder.CreateIndex(
                name: "IX_cases_resolved_at",
                table: "cases",
                column: "resolved_at");

            migrationBuilder.CreateIndex(
                name: "IX_cases_system_component_id",
                table: "cases",
                column: "system_component_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_system_id",
                table: "cases",
                column: "system_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_relationships_source_case_id_related_case_id_relations~",
                table: "case_relationships",
                columns: new[] { "source_case_id", "related_case_id", "relationship_type" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_customer_orders_customer_order_id",
                table: "cases",
                column: "customer_order_id",
                principalTable: "customer_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_customer_sites_customer_site_id",
                table: "cases",
                column: "customer_site_id",
                principalTable: "customer_sites",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_system_components_system_component_id",
                table: "cases",
                column: "system_component_id",
                principalTable: "system_components",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_systems_system_id",
                table: "cases",
                column: "system_id",
                principalTable: "systems",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cases_customer_orders_customer_order_id",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_customer_sites_customer_site_id",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_system_components_system_component_id",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_systems_system_id",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_customer_order_id",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_customer_site_id",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_first_response_at",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_resolved_at",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_system_component_id",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_system_id",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_case_relationships_source_case_id_related_case_id_relations~",
                table: "case_relationships");

            migrationBuilder.DropColumn(
                name: "customer_order_id",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "customer_site_id",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "system_component_id",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "system_id",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "display_order",
                table: "case_templates");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "case_relationships");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "case_relationships");
        }
    }
}
