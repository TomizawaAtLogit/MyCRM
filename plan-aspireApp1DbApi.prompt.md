Plan: Add Postgres Web API Project — AspireApp1.DbApi

Summary
- Create a new ASP.NET Core Web API project `AspireApp1.DbApi` (TargetFramework: net9.0) that exposes a Projects API and stores data in PostgreSQL using EF Core (Npgsql provider). Register the service in `AspireApp1.AppHost` so it runs alongside `apiservice` and `web`.

Goals
- Separate DB-backed API service for project management.
- Use EF Core + Npgsql with migrations managed via `dotnet ef` and AppHost as startup.
- Provide a simple repository abstraction and controllers, and expose Swagger.

High-level Implementation Steps
1) Create project file
- Add `AspireApp1.DbApi/AspireApp1.DbApi.csproj` (SDK Microsoft.NET.Sdk.Web, TargetFramework `net9.0`).
- PackageReferences: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`, `Swashbuckle.AspNetCore`.

2) Add project reference to AppHost
- Edit `AspireApp1.AppHost/AspireApp1.AppHost.csproj` and add `<ProjectReference Include="..\\AspireApp1.DbApi\\AspireApp1.DbApi.csproj" />` so AppHost can orchestrate the service.

3) Register in AppHost program
- Edit `AspireApp1.AppHost/Program.cs` and add a registration similar to existing ones: `builder.AddProject<Projects.AspireApp1_DbApi>("dbapi");`.

4) Program skeleton for DbApi
- Create `AspireApp1.DbApi/Program.cs` following existing project patterns: call `builder.AddServiceDefaults();` then set up DI, controllers, Swagger/OpenAPI, and map endpoints.

5) Add EF models and DbContext
- Create `AspireApp1.DbApi/Data/ProjectDbContext.cs` (class `ProjectDbContext : DbContext`).
- Add entity classes under `AspireApp1.DbApi/Models/` (e.g., `Project.cs`, with necessary properties).

6) Repository layer
- Add `AspireApp1.DbApi/Repositories/IProjectRepository.cs` and `AspireApp1.DbApi/Repositories/ProjectRepository.cs` implementing CRUD over `ProjectDbContext` (symbols: `IProjectRepository`, `ProjectRepository`).
- Register repository with DI in `Program.cs` (`builder.Services.AddScoped<IProjectRepository, ProjectRepository>();`).

7) API Controllers
- Add `AspireApp1.DbApi/Controllers/ProjectsController.cs` that consumes `IProjectRepository` and exposes routes (GET, POST, PUT, DELETE).
- Enable OpenAPI via `AddSwaggerGen()` and map Swagger UI in `Program.cs`.

8) Configuration and connection string
- Add `AspireApp1.DbApi/appsettings.json` with:
  ConnectionStrings:
    DefaultConnection: "Host=localhost;Database=aspire_db;Username=postgres;Password=YourPassword"
- Document that AppHost (or environment/secrets) should override production credentials.

9) Register DbContext
- In `AspireApp1.DbApi/Program.cs` add:
  `builder.Services.AddDbContext<ProjectDbContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));`

10) Migrations and run instructions
- From solution root, create and apply migrations with AppHost as startup:

```pwsh
dotnet ef migrations add InitialCreate --project AspireApp1.DbApi --startup-project AspireApp1.AppHost --context ProjectDbContext
dotnet ef database update --project AspireApp1.DbApi --startup-project AspireApp1.AppHost --context ProjectDbContext
```

Notes, Risks, and Questions
- No existing EF/DB usage found in the repo; adding EF Core introduces a new dependency surface.
- AppHost manages project lifecycles and service discovery; register `dbapi` there and update `AspireApp1.Web` HttpClient registrations if the frontend needs to call the new service (e.g., `https+http://dbapi`).
- Migrations require design-time services; using `AspireApp1.AppHost` as `--startup-project` is recommended so configuration matches runtime.
- Do you prefer a separate `AspireApp1.DbApi` service (recommended) or adding DB to `AspireApp1.ApiService` instead?
- Would you like a `docker-compose.yml` for local Postgres added now?

Next steps
- On approval I will scaffold the minimal files (Program.cs, DbContext, one model, repository, controller, appsettings) and the csproj; then run `dotnet restore` and create the initial migration.

