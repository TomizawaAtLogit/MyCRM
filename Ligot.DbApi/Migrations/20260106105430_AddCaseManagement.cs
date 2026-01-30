using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ligot.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "case_templates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    issue_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    default_priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title_template = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description_template = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cases",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    issue_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    assigned_to_user_id = table.Column<int>(type: "integer", nullable: true),
                    resolution_notes = table.Column<string>(type: "text", nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    first_response_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sla_deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cases", x => x.id);
                    table.ForeignKey(
                        name: "FK_cases_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cases_users_assigned_to_user_id",
                        column: x => x.assigned_to_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "sla_thresholds",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    response_time_hours = table.Column<int>(type: "integer", nullable: false),
                    resolution_time_hours = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sla_thresholds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "case_activities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    case_id = table.Column<int>(type: "integer", nullable: false),
                    activity_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    next_action = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    activity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    performed_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    previous_assigned_to_user_id = table.Column<int>(type: "integer", nullable: true),
                    new_assigned_to_user_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_activities", x => x.id);
                    table.ForeignKey(
                        name: "FK_case_activities_cases_case_id",
                        column: x => x.case_id,
                        principalTable: "cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "case_relationships",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source_case_id = table.Column<int>(type: "integer", nullable: false),
                    related_case_id = table.Column<int>(type: "integer", nullable: false),
                    relationship_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_relationships", x => x.id);
                    table.ForeignKey(
                        name: "FK_case_relationships_cases_related_case_id",
                        column: x => x.related_case_id,
                        principalTable: "cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_case_relationships_cases_source_case_id",
                        column: x => x.source_case_id,
                        principalTable: "cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_case_activities_activity_date",
                table: "case_activities",
                column: "activity_date");

            migrationBuilder.CreateIndex(
                name: "IX_case_activities_case_id",
                table: "case_activities",
                column: "case_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_relationships_related_case_id",
                table: "case_relationships",
                column: "related_case_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_relationships_source_case_id",
                table: "case_relationships",
                column: "source_case_id");

            migrationBuilder.CreateIndex(
                name: "IX_case_templates_is_active",
                table: "case_templates",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_case_templates_issue_type",
                table: "case_templates",
                column: "issue_type");

            migrationBuilder.CreateIndex(
                name: "IX_cases_assigned_to_user_id",
                table: "cases",
                column: "assigned_to_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_customer_id",
                table: "cases",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_cases_due_date",
                table: "cases",
                column: "due_date");

            migrationBuilder.CreateIndex(
                name: "IX_cases_priority",
                table: "cases",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "IX_cases_status",
                table: "cases",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_sla_thresholds_is_active",
                table: "sla_thresholds",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_sla_thresholds_priority",
                table: "sla_thresholds",
                column: "priority");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "case_activities");

            migrationBuilder.DropTable(
                name: "case_relationships");

            migrationBuilder.DropTable(
                name: "case_templates");

            migrationBuilder.DropTable(
                name: "sla_thresholds");

            migrationBuilder.DropTable(
                name: "cases");
        }
    }
}

