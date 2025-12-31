using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspireApp1.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class MoveCustomerIdToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add nullable customer_id column to projects table
            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "projects",
                type: "integer",
                nullable: true);

            // Step 2: Add status column with default value
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "projects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Wip");

            // Step 3: Validate and copy customer_id from project_activities to projects
            // This SQL identifies projects with conflicts and fails if any are found
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    conflict_count INTEGER;
                    conflict_projects TEXT;
                BEGIN
                    -- Check for projects with multiple distinct customer_ids or null customer_ids
                    SELECT COUNT(*), STRING_AGG(DISTINCT project_id::TEXT, ', ')
                    INTO conflict_count, conflict_projects
                    FROM (
                        SELECT 
                            p.id AS project_id,
                            COUNT(DISTINCT pa.customer_id) AS distinct_customer_count,
                            COUNT(*) FILTER (WHERE pa.customer_id IS NULL) AS null_count
                        FROM projects p
                        LEFT JOIN project_activities pa ON pa.project_id = p.id
                        GROUP BY p.id
                        HAVING 
                            COUNT(DISTINCT pa.customer_id) > 1  -- Multiple different customers
                            OR (COUNT(DISTINCT pa.customer_id) = 1 AND COUNT(*) FILTER (WHERE pa.customer_id IS NULL) > 0)  -- Mix of NULL and non-NULL
                            OR (COUNT(*) > 0 AND COUNT(DISTINCT pa.customer_id) = 0)  -- All activities have NULL customer_id
                            OR COUNT(pa.id) = 0  -- No activities at all
                    ) conflicts;
                    
                    IF conflict_count > 0 THEN
                        RAISE EXCEPTION 'Migration failed: % projects have conflicting or missing customer assignments. Project IDs: %. Please run validation-script.sql and manually resolve conflicts before migrating.', 
                            conflict_count, conflict_projects;
                    END IF;
                    
                    -- Copy customer_id from project_activities to projects
                    UPDATE projects p
                    SET customer_id = (
                        SELECT customer_id 
                        FROM project_activities pa 
                        WHERE pa.project_id = p.id 
                        AND pa.customer_id IS NOT NULL
                        LIMIT 1
                    );
                END $$;
            ");

            // Step 4: Make customer_id NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "projects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // Step 5: Create index and foreign key for projects.customer_id
            migrationBuilder.CreateIndex(
                name: "IX_projects_customer_id",
                table: "projects",
                column: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_customers_customer_id",
                table: "projects",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // Step 6: Drop foreign key and index from project_activities
            migrationBuilder.DropForeignKey(
                name: "FK_project_activities_customers_customer_id",
                table: "project_activities");

            migrationBuilder.DropIndex(
                name: "IX_project_activities_customer_id",
                table: "project_activities");

            // Step 7: Drop customer_id column from project_activities
            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "project_activities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add nullable customer_id column back to project_activities
            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "project_activities",
                type: "integer",
                nullable: true);

            // Step 2: Copy customer_id from projects back to project_activities
            migrationBuilder.Sql(@"
                UPDATE project_activities pa
                SET customer_id = p.customer_id
                FROM projects p
                WHERE pa.project_id = p.id;
            ");

            // Step 3: Create index and foreign key for project_activities.customer_id
            migrationBuilder.CreateIndex(
                name: "IX_project_activities_customer_id",
                table: "project_activities",
                column: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_project_activities_customers_customer_id",
                table: "project_activities",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            // Step 4: Drop foreign key and index from projects
            migrationBuilder.DropForeignKey(
                name: "FK_projects_customers_customer_id",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "IX_projects_customer_id",
                table: "projects");

            // Step 5: Drop columns from projects
            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "status",
                table: "projects");
        }
    }
}
