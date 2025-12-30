using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AspireApp1.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSystemsToComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create new systems table
            migrationBuilder.CreateTable(
                name: "systems",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    system_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    installation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systems", x => x.id);
                    table.ForeignKey(
                        name: "FK_systems_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create new system_components table
            migrationBuilder.CreateTable(
                name: "system_components",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    system_id = table.Column<int>(type: "integer", nullable: false),
                    component_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    model = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    serial_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    warranty_expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_components", x => x.id);
                    table.ForeignKey(
                        name: "FK_system_components_systems_system_id",
                        column: x => x.system_id,
                        principalTable: "systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate existing data from customer_systems to new structure
            migrationBuilder.Sql(@"
                -- First, create systems from unique system names per customer
                INSERT INTO systems (customer_id, system_name, location, installation_date, description, created_at, updated_at)
                SELECT DISTINCT 
                    customer_id,
                    system_name,
                    location,
                    installation_date,
                    description,
                    created_at,
                    updated_at
                FROM customer_systems
                GROUP BY customer_id, system_name, location, installation_date, description, created_at, updated_at;

                -- Then, create components for each old system record
                INSERT INTO system_components (system_id, component_type, manufacturer, model, serial_number, warranty_expiration, description, created_at, updated_at)
                SELECT 
                    s.id,
                    COALESCE(cs.component_type, 'Unknown'),
                    cs.manufacturer,
                    cs.model,
                    cs.serial_number,
                    cs.warranty_expiration,
                    cs.description,
                    cs.created_at,
                    cs.updated_at
                FROM customer_systems cs
                INNER JOIN systems s ON cs.customer_id = s.customer_id AND cs.system_name = s.system_name
                WHERE cs.component_type IS NOT NULL OR cs.manufacturer IS NOT NULL OR cs.model IS NOT NULL OR cs.serial_number IS NOT NULL;
            ");

            // Drop old customer_systems table
            migrationBuilder.DropTable(
                name: "customer_systems");

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_systems_customer_id",
                table: "systems",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_system_components_system_id",
                table: "system_components",
                column: "system_id");

            migrationBuilder.CreateIndex(
                name: "IX_system_components_serial_number",
                table: "system_components",
                column: "serial_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate old customer_systems table
            migrationBuilder.CreateTable(
                name: "customer_systems",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    system_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    component_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    model = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    serial_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    installation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    warranty_expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_systems", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_systems_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate data back from new structure to old
            migrationBuilder.Sql(@"
                INSERT INTO customer_systems (customer_id, system_name, component_type, manufacturer, model, serial_number, location, installation_date, warranty_expiration, description, created_at, updated_at)
                SELECT 
                    s.customer_id,
                    s.system_name,
                    sc.component_type,
                    sc.manufacturer,
                    sc.model,
                    sc.serial_number,
                    s.location,
                    s.installation_date,
                    sc.warranty_expiration,
                    s.description,
                    s.created_at,
                    s.updated_at
                FROM systems s
                LEFT JOIN system_components sc ON s.id = sc.system_id;
            ");

            // Drop new tables
            migrationBuilder.DropTable(
                name: "system_components");

            migrationBuilder.DropTable(
                name: "systems");

            // Recreate index
            migrationBuilder.CreateIndex(
                name: "IX_customer_systems_customer_id",
                table: "customer_systems",
                column: "customer_id");
        }
    }
}
