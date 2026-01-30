using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ligot.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dashboard_metrics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    snapshot_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: true),
                    customer_id = table.Column<int>(type: "integer", nullable: true),
                    total_presales_proposals = table.Column<int>(type: "integer", nullable: false),
                    active_presales_proposals = table.Column<int>(type: "integer", nullable: false),
                    presales_proposals_by_stage_identification = table.Column<int>(type: "integer", nullable: false),
                    presales_proposals_by_stage_qualification = table.Column<int>(type: "integer", nullable: false),
                    presales_proposals_by_stage_proposal = table.Column<int>(type: "integer", nullable: false),
                    presales_proposals_by_stage_negotiation = table.Column<int>(type: "integer", nullable: false),
                    presales_proposals_by_stage_closed_won = table.Column<int>(type: "integer", nullable: false),
                    presales_proposals_by_stage_closed_lost = table.Column<int>(type: "integer", nullable: false),
                    total_cases = table.Column<int>(type: "integer", nullable: false),
                    open_cases = table.Column<int>(type: "integer", nullable: false),
                    in_progress_cases = table.Column<int>(type: "integer", nullable: false),
                    resolved_cases = table.Column<int>(type: "integer", nullable: false),
                    closed_cases = table.Column<int>(type: "integer", nullable: false),
                    critical_priority_cases = table.Column<int>(type: "integer", nullable: false),
                    high_priority_cases = table.Column<int>(type: "integer", nullable: false),
                    medium_priority_cases = table.Column<int>(type: "integer", nullable: false),
                    low_priority_cases = table.Column<int>(type: "integer", nullable: false),
                    case_resolution_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    sla_compliance_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    average_resolution_time_hours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    cases_resolved_within_sla = table.Column<int>(type: "integer", nullable: false),
                    cases_resolved_outside_sla = table.Column<int>(type: "integer", nullable: false),
                    total_projects = table.Column<int>(type: "integer", nullable: false),
                    active_projects = table.Column<int>(type: "integer", nullable: false),
                    completed_projects = table.Column<int>(type: "integer", nullable: false),
                    on_hold_projects = table.Column<int>(type: "integer", nullable: false),
                    project_completion_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dashboard_metrics", x => x.id);
                    table.ForeignKey(
                        name: "FK_dashboard_metrics_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_dashboard_metrics_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dashboard_metrics_customer_id",
                table: "dashboard_metrics",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_dashboard_metrics_role_id",
                table: "dashboard_metrics",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_dashboard_metrics_snapshot_date",
                table: "dashboard_metrics",
                column: "snapshot_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dashboard_metrics");
        }
    }
}

