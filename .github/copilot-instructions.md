# Copilot Instructions for Ligot

## Build, Test, and Run

```bash
# Restore and build
dotnet restore
dotnet build

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "ClassName=PreSalesProposalUpdateTests"

# Run a single test method
dotnet test --filter "FullyQualifiedName=Ligot.Tests.PreSalesProposalUpdateTests.Put_AssignedToUserIdUpdated_PersistsAssignmentAndCreatesActivity"

# Run the app (Aspire orchestrator starts all services)
dotnet run --project Ligot.AppHost

# Start local PostgreSQL + Adminer (required before running)
docker compose -f docker-compose.postgres.yml up -d

# EF migrations (must use AppHost as startup project)
dotnet ef migrations add <MigrationName> --project Ligot.DbApi --startup-project Ligot.AppHost --context ProjectDbContext
dotnet ef database update --project Ligot.DbApi --startup-project Ligot.AppHost --context ProjectDbContext
```

## Architecture

This is a **.NET 10 + .NET Aspire** solution with three runtime services orchestrated by `Ligot.AppHost`:

- **`Ligot.DbApi`** (assembly: `Ligot.BackEnd`) — the primary backend API. ASP.NET Core Web API using EF Core + Npgsql against PostgreSQL. Exposes all domain endpoints under `api/[controller]`.
- **`Ligot.Web`** (assembly: `Ligot.FrontEnd`) — Blazor SSR frontend using Razor Components (interactive server mode). Calls DbApi via strongly-typed `HttpClient` wrappers.
- **`Ligot.ApiService`** — a minimal placeholder service (no domain logic yet).
- **`Ligot.ServiceDefaults`** — shared Aspire service defaults (observability, health checks) added via `builder.AddServiceDefaults()`.

The frontend discovers DbApi via Aspire service name `"dbapi"` (`https+http://dbapi`). Override with `DbApiBaseUrl` in `appsettings.Development.Local.json` for standalone local runs without the AppHost.

## Key Conventions

### Repository pattern in DbApi
Every domain entity has an interface (`IXxxRepository`) and implementation (`XxxRepository`) in `Ligot.DbApi/Repositories/`. Register new repositories as `AddScoped` in `Ligot.DbApi/Program.cs`.

### DTOs as records
All DTOs in `Ligot.DbApi/DTOs/` are C# records. Create/update DTOs are typically separate (e.g., `CreateProjectDto`, `UpdatePreSalesProposalDto`).

### Controller base class
All controllers that need to know the current user or write audit logs must inherit from `AuditableControllerBase` (not `ControllerBase` directly). Call `GetCurrentUserInfoAsync()` to resolve the current username and user ID. When Windows auth is disabled locally, it falls back to `Environment.UserName`.

### Audit logging
Call `_auditService.LogActionAsync(username, userId, action, entityType, entityId, entity)` for all Create/Read/Update/Delete operations. Action strings are `"Create"`, `"Read"`, `"Update"`, `"Delete"`.

### Role/permission model
Roles store permissions as a CSV string on the `PagePermissions` column in the format `"Page:PermissionLevel"` (e.g., `"Projects:FullControl,Customers:ReadOnly"`). Check permissions using `PagePermissionHelper.HasPagePermission()`. Permission levels used are `"FullControl"` and `"ReadOnly"`. The legacy format `"Page"` (no level) is treated as `"FullControl"`.

### Windows Authentication disabled for local dev
Both `Ligot.DbApi/Program.cs` and `Ligot.Web/Program.cs` have authentication/authorization blocks commented out. Do not uncomment these for local development. Production deployment re-enables Windows Negotiate auth and role-based authorization policies (`AdminOnly`, `SupportOnly`, `PreSalesOnly`).

### Npgsql DateTime behavior
`AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)` is set in DbApi. Use `DateTime` (not `DateTimeOffset`) for all timestamp properties on EF models.

### Auto-migration at startup
DbApi calls `db.Database.Migrate()` at startup and seeds default roles, an admin user, and SLA thresholds if the `Users` table is empty. Do not remove this block; it is the local-dev migration mechanism.

### Frontend typed HTTP clients
Each API domain area has its own `XxxApiClient` class in `Ligot.Web/`. All clients target `https+http://dbapi` via Aspire discovery (overridable with `DbApiBaseUrl`), use `UseDefaultCredentials = true`, and attach `CookieForwardingHandler`. When adding a new API domain, follow the same registration pattern in `Ligot.Web/Program.cs`.

### Local configuration override
`appsettings.Development.Local.json` is gitignored and loaded optionally by both AppHost and Web. Use it for local overrides (e.g., `DbApiBaseUrl`).

### Tests
- **Integration tests** (`WebTests.cs`) use `DistributedApplicationTestingBuilder` — these require Docker and a running Postgres.
- **Unit tests** (`PreSalesProposalUpdateTests.cs`) use EF Core `InMemoryDatabase` and instantiate controllers directly — no infrastructure needed.
- Test framework is **MSTest**.

## CI/CD

GitHub Actions workflows in `.github/workflows/`:
- `ci-build.yml` — runs on push/PR to `main`/`develop`: restore, build Release, test, publish both apps.
- `deploy-staging.yml` / `deploy-production.yml` — manual dispatch, deploys to Azure App Service with staging-slot swap strategy.
- `rollback.yml` / `deploy-infrastructure.yml` — infrastructure and rollback operations.

Production deploys to Azure App Service (resource group `rg-mycrm-production`). Required secrets: `AZURE_CREDENTIALS`, `PRODUCTION_DB_CONNECTION_STRING`. See `.github/SECRETS.md` for the full secrets list.
