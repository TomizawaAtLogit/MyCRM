using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ligot.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddPreSalesFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "requirement_definitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirement_definitions", x => x.id);
                    table.ForeignKey(
                        name: "FK_requirement_definitions_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "presales_proposals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    requirement_definition_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    stage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    assigned_to_user_id = table.Column<int>(type: "integer", nullable: true),
                    estimated_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    probability_percentage = table.Column<int>(type: "integer", nullable: true),
                    expected_close_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_presales_proposals", x => x.id);
                    table.ForeignKey(
                        name: "FK_presales_proposals_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_presales_proposals_requirement_definitions_requirement_defi~",
                        column: x => x.requirement_definition_id,
                        principalTable: "requirement_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_presales_proposals_users_assigned_to_user_id",
                        column: x => x.assigned_to_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "presales_activities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    presales_proposal_id = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_presales_activities", x => x.id);
                    table.ForeignKey(
                        name: "FK_presales_activities_presales_proposals_presales_proposal_id",
                        column: x => x.presales_proposal_id,
                        principalTable: "presales_proposals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_presales_activities_activity_date",
                table: "presales_activities",
                column: "activity_date");

            migrationBuilder.CreateIndex(
                name: "IX_presales_activities_presales_proposal_id",
                table: "presales_activities",
                column: "presales_proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_presales_proposals_assigned_to_user_id",
                table: "presales_proposals",
                column: "assigned_to_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_presales_proposals_customer_id",
                table: "presales_proposals",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_presales_proposals_expected_close_date",
                table: "presales_proposals",
                column: "expected_close_date");

            migrationBuilder.CreateIndex(
                name: "IX_presales_proposals_requirement_definition_id",
                table: "presales_proposals",
                column: "requirement_definition_id");

            migrationBuilder.CreateIndex(
                name: "IX_presales_proposals_stage",
                table: "presales_proposals",
                column: "stage");

            migrationBuilder.CreateIndex(
                name: "IX_presales_proposals_status",
                table: "presales_proposals",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_definitions_category",
                table: "requirement_definitions",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_definitions_customer_id",
                table: "requirement_definitions",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_definitions_status",
                table: "requirement_definitions",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "presales_activities");

            migrationBuilder.DropTable(
                name: "presales_proposals");

            migrationBuilder.DropTable(
                name: "requirement_definitions");
        }
    }
}

