using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AspireApp1.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class MoveLocationToComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_databases_customers_CustomerId",
                table: "customer_databases");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_orders_customers_CustomerId",
                table: "customer_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_sites_customers_CustomerId",
                table: "customer_sites");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_systems_customers_CustomerId",
                table: "customer_systems");

            migrationBuilder.DropForeignKey(
                name: "FK_project_activities_customers_CustomerId",
                table: "project_activities");

            migrationBuilder.DropForeignKey(
                name: "FK_project_activities_projects_ProjectId",
                table: "project_activities");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WindowsUsername",
                table: "users",
                newName: "windows_username");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "users",
                newName: "display_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_users_WindowsUsername",
                table: "users",
                newName: "IX_users_windows_username");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_roles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_roles",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user_roles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "user_roles",
                newName: "assigned_at");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_UserId_RoleId",
                table: "user_roles",
                newName: "IX_user_roles_user_id_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                newName: "IX_user_roles_role_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "roles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "roles",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PagePermissions",
                table: "roles",
                newName: "page_permissions");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "roles",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_roles_Name",
                table: "roles",
                newName: "IX_roles_name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "projects",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "projects",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "projects",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "projects",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "project_activities",
                newName: "summary");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "project_activities",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "project_activities",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "project_activities",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "project_activities",
                newName: "project_id");

            migrationBuilder.RenameColumn(
                name: "PerformedBy",
                table: "project_activities",
                newName: "performed_by");

            migrationBuilder.RenameColumn(
                name: "NextAction",
                table: "project_activities",
                newName: "next_action");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "project_activities",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "project_activities",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ActivityType",
                table: "project_activities",
                newName: "activity_type");

            migrationBuilder.RenameColumn(
                name: "ActivityDate",
                table: "project_activities",
                newName: "activity_date");

            migrationBuilder.RenameIndex(
                name: "IX_project_activities_ProjectId",
                table: "project_activities",
                newName: "IX_project_activities_project_id");

            migrationBuilder.RenameIndex(
                name: "IX_project_activities_CustomerId",
                table: "project_activities",
                newName: "IX_project_activities_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_project_activities_ActivityDate",
                table: "project_activities",
                newName: "IX_project_activities_activity_date");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "customers",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "customers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "customers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "customers",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customers",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customers",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContactPerson",
                table: "customers",
                newName: "contact_person");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "customer_systems",
                newName: "model");

            migrationBuilder.RenameColumn(
                name: "Manufacturer",
                table: "customer_systems",
                newName: "manufacturer");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "customer_systems",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "customer_systems",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customer_systems",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WarrantyExpiration",
                table: "customer_systems",
                newName: "warranty_expiration");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customer_systems",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "customer_systems",
                newName: "system_name");

            migrationBuilder.RenameColumn(
                name: "SerialNumber",
                table: "customer_systems",
                newName: "serial_number");

            migrationBuilder.RenameColumn(
                name: "InstallationDate",
                table: "customer_systems",
                newName: "installation_date");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "customer_systems",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customer_systems",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ComponentType",
                table: "customer_systems",
                newName: "component_type");

            migrationBuilder.RenameIndex(
                name: "IX_customer_systems_CustomerId",
                table: "customer_systems",
                newName: "IX_customer_systems_customer_id");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "customer_sites",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "customer_sites",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "customer_sites",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "customer_sites",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customer_sites",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customer_sites",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SiteName",
                table: "customer_sites",
                newName: "site_name");

            migrationBuilder.RenameColumn(
                name: "PostCode",
                table: "customer_sites",
                newName: "post_code");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "customer_sites",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customer_sites",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContactPerson",
                table: "customer_sites",
                newName: "contact_person");

            migrationBuilder.RenameIndex(
                name: "IX_customer_sites_CustomerId",
                table: "customer_sites",
                newName: "IX_customer_sites_customer_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "customer_orders",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "customer_orders",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customer_orders",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customer_orders",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "customer_orders",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                table: "customer_orders",
                newName: "order_number");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "customer_orders",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "customer_orders",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customer_orders",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContractValue",
                table: "customer_orders",
                newName: "contract_value");

            migrationBuilder.RenameColumn(
                name: "ContractType",
                table: "customer_orders",
                newName: "contract_type");

            migrationBuilder.RenameColumn(
                name: "BillingFrequency",
                table: "customer_orders",
                newName: "billing_frequency");

            migrationBuilder.RenameIndex(
                name: "IX_customer_orders_OrderNumber",
                table: "customer_orders",
                newName: "IX_customer_orders_order_number");

            migrationBuilder.RenameIndex(
                name: "IX_customer_orders_CustomerId",
                table: "customer_orders",
                newName: "IX_customer_orders_customer_id");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "customer_databases",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "Port",
                table: "customer_databases",
                newName: "port");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "customer_databases",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customer_databases",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customer_databases",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ServerName",
                table: "customer_databases",
                newName: "server_name");

            migrationBuilder.RenameColumn(
                name: "DatabaseType",
                table: "customer_databases",
                newName: "database_type");

            migrationBuilder.RenameColumn(
                name: "DatabaseName",
                table: "customer_databases",
                newName: "database_name");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "customer_databases",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customer_databases",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_customer_databases_CustomerId",
                table: "customer_databases",
                newName: "IX_customer_databases_customer_id");

            migrationBuilder.CreateTable(
                name: "systems",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    system_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
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
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_system_components_serial_number",
                table: "system_components",
                column: "serial_number");

            migrationBuilder.CreateIndex(
                name: "IX_system_components_system_id",
                table: "system_components",
                column: "system_id");

            migrationBuilder.CreateIndex(
                name: "IX_systems_customer_id",
                table: "systems",
                column: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_databases_customers_customer_id",
                table: "customer_databases",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_orders_customers_customer_id",
                table: "customer_orders",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_sites_customers_customer_id",
                table: "customer_sites",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_systems_customers_customer_id",
                table: "customer_systems",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_project_activities_customers_customer_id",
                table: "project_activities",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_project_activities_projects_project_id",
                table: "project_activities",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_databases_customers_customer_id",
                table: "customer_databases");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_orders_customers_customer_id",
                table: "customer_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_sites_customers_customer_id",
                table: "customer_sites");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_systems_customers_customer_id",
                table: "customer_systems");

            migrationBuilder.DropForeignKey(
                name: "FK_project_activities_customers_customer_id",
                table: "project_activities");

            migrationBuilder.DropForeignKey(
                name: "FK_project_activities_projects_project_id",
                table: "project_activities");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles");

            migrationBuilder.DropTable(
                name: "system_components");

            migrationBuilder.DropTable(
                name: "systems");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "windows_username",
                table: "users",
                newName: "WindowsUsername");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "users",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "users",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_users_windows_username",
                table: "users",
                newName: "IX_users_WindowsUsername");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user_roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user_roles",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "user_roles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "assigned_at",
                table: "user_roles",
                newName: "AssignedAt");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_user_id_role_id",
                table: "user_roles",
                newName: "IX_user_roles_UserId_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                newName: "IX_user_roles_RoleId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "roles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "roles",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "page_permissions",
                table: "roles",
                newName: "PagePermissions");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "roles",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_roles_name",
                table: "roles",
                newName: "IX_roles_Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "projects",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "projects",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "projects",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "projects",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "summary",
                table: "project_activities",
                newName: "Summary");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "project_activities",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "project_activities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "project_activities",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "project_activities",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "performed_by",
                table: "project_activities",
                newName: "PerformedBy");

            migrationBuilder.RenameColumn(
                name: "next_action",
                table: "project_activities",
                newName: "NextAction");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "project_activities",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "project_activities",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "activity_type",
                table: "project_activities",
                newName: "ActivityType");

            migrationBuilder.RenameColumn(
                name: "activity_date",
                table: "project_activities",
                newName: "ActivityDate");

            migrationBuilder.RenameIndex(
                name: "IX_project_activities_project_id",
                table: "project_activities",
                newName: "IX_project_activities_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_project_activities_customer_id",
                table: "project_activities",
                newName: "IX_project_activities_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_project_activities_activity_date",
                table: "project_activities",
                newName: "IX_project_activities_ActivityDate");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "customers",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "customers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "customers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "customers",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "customers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "customers",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "customers",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "contact_person",
                table: "customers",
                newName: "ContactPerson");

            migrationBuilder.RenameColumn(
                name: "model",
                table: "customer_systems",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "manufacturer",
                table: "customer_systems",
                newName: "Manufacturer");

            migrationBuilder.RenameColumn(
                name: "location",
                table: "customer_systems",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "customer_systems",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "customer_systems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "warranty_expiration",
                table: "customer_systems",
                newName: "WarrantyExpiration");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "customer_systems",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "system_name",
                table: "customer_systems",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "serial_number",
                table: "customer_systems",
                newName: "SerialNumber");

            migrationBuilder.RenameColumn(
                name: "installation_date",
                table: "customer_systems",
                newName: "InstallationDate");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "customer_systems",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "customer_systems",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "component_type",
                table: "customer_systems",
                newName: "ComponentType");

            migrationBuilder.RenameIndex(
                name: "IX_customer_systems_customer_id",
                table: "customer_systems",
                newName: "IX_customer_systems_CustomerId");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "customer_sites",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "customer_sites",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "customer_sites",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "customer_sites",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "customer_sites",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "customer_sites",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "site_name",
                table: "customer_sites",
                newName: "SiteName");

            migrationBuilder.RenameColumn(
                name: "post_code",
                table: "customer_sites",
                newName: "PostCode");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "customer_sites",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "customer_sites",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "contact_person",
                table: "customer_sites",
                newName: "ContactPerson");

            migrationBuilder.RenameIndex(
                name: "IX_customer_sites_customer_id",
                table: "customer_sites",
                newName: "IX_customer_sites_CustomerId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "customer_orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "customer_orders",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "customer_orders",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "customer_orders",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "customer_orders",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "order_number",
                table: "customer_orders",
                newName: "OrderNumber");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "customer_orders",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "customer_orders",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "customer_orders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "contract_value",
                table: "customer_orders",
                newName: "ContractValue");

            migrationBuilder.RenameColumn(
                name: "contract_type",
                table: "customer_orders",
                newName: "ContractType");

            migrationBuilder.RenameColumn(
                name: "billing_frequency",
                table: "customer_orders",
                newName: "BillingFrequency");

            migrationBuilder.RenameIndex(
                name: "IX_customer_orders_order_number",
                table: "customer_orders",
                newName: "IX_customer_orders_OrderNumber");

            migrationBuilder.RenameIndex(
                name: "IX_customer_orders_customer_id",
                table: "customer_orders",
                newName: "IX_customer_orders_CustomerId");

            migrationBuilder.RenameColumn(
                name: "version",
                table: "customer_databases",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "port",
                table: "customer_databases",
                newName: "Port");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "customer_databases",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "customer_databases",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "customer_databases",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "server_name",
                table: "customer_databases",
                newName: "ServerName");

            migrationBuilder.RenameColumn(
                name: "database_type",
                table: "customer_databases",
                newName: "DatabaseType");

            migrationBuilder.RenameColumn(
                name: "database_name",
                table: "customer_databases",
                newName: "DatabaseName");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "customer_databases",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "customer_databases",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_customer_databases_customer_id",
                table: "customer_databases",
                newName: "IX_customer_databases_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_databases_customers_CustomerId",
                table: "customer_databases",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_orders_customers_CustomerId",
                table: "customer_orders",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_sites_customers_CustomerId",
                table: "customer_sites",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_systems_customers_CustomerId",
                table: "customer_systems",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_project_activities_customers_CustomerId",
                table: "project_activities",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_project_activities_projects_ProjectId",
                table: "project_activities",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_RoleId",
                table: "user_roles",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_UserId",
                table: "user_roles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
