AspireApp1.DbApi

This project is a minimal ASP.NET Core Web API that uses Entity Framework Core with the Npgsql provider to store project management data in PostgreSQL.

Quick start (from solution root):

```pwsh
dotnet restore
# create migrations (requires dotnet-ef and AppHost as startup project)
# dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project AspireApp1.DbApi --startup-project AspireApp1.AppHost --context ProjectDbContext
dotnet ef database update --project AspireApp1.DbApi --startup-project AspireApp1.AppHost --context ProjectDbContext
```

Connection string is located in `appsettings.json` (development). Use AppHost environment variables or user secrets to override in local/dev runs.
