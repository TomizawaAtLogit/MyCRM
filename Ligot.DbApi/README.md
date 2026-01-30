Ligot.DbApi

This project is a minimal ASP.NET Core Web API that uses Entity Framework Core with the Npgsql provider to store project management data in PostgreSQL.

Quick start (from solution root):

```pwsh
dotnet restore
# create migrations (requires dotnet-ef and AppHost as startup project)
# dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project Ligot.DbApi --startup-project Ligot.AppHost --context ProjectDbContext
dotnet ef database update --project Ligot.DbApi --startup-project Ligot.AppHost --context ProjectDbContext
```

Connection string is located in `appsettings.json` (development). Use AppHost environment variables or user secrets to override in local/dev runs.
 
Local Postgres using Docker Compose

A simple docker-compose file is included at the repo root: `docker-compose.postgres.yml`. It provides Postgres 15 and Adminer for local development.

Start Postgres + Adminer:

```pwsh
docker compose -f docker-compose.postgres.yml up -d
```

- Postgres is exposed on host port `5432`.
- Adminer UI is available at http://localhost:8080 (login: server `localhost`, username `postgres`, password `postgrespw`, database `aspire_db`).

Connection string example for local development (in `Ligot.DbApi/appsettings.json`):

```
Host=localhost;Port=5432;Database=aspire_db;Username=postgres;Password=postgrespw
```

After starting Postgres, create and apply EF migrations (from solution root):

```pwsh
# ensure dotnet-ef is available
dotnet tool install --global dotnet-ef --version 8.*

# add and apply migration
dotnet ef migrations add InitialCreate --project Ligot.DbApi --context ProjectDbContext
dotnet ef database update --project Ligot.DbApi --context ProjectDbContext
```

