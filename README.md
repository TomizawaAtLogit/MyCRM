# Ligot ✅

**Ligot** is a sample/production-ready .NET web application used for project management scenarios. It contains backend APIs, a web frontend, a database API using EF Core with PostgreSQL, and CI/CD automation for deploying to Azure.

---

## 🔧 Tech stack

- .NET 10 (net10.0)
- ASP.NET Core Web APIs
- Entity Framework Core (Npgsql provider)
- PostgreSQL (local dev via Docker Compose)
- GitHub Actions for CI/CD

---

## 🚀 Quick start (local development)

Prerequisites:
- .NET SDK 10
- Docker (for local Postgres)
- (Optional) `dotnet-ef` for migrations: `dotnet tool install --global dotnet-ef`

1. Clone the repo:

```bash
git clone <repo-url>
cd Ligot
```

2. Start local Postgres + Adminer (provided):

```bash
docker compose -f docker-compose.postgres.yml up -d
```

Adminer UI: http://localhost:8080 (server `localhost`, username `postgres`, password `postgrespw`, database `aspire_db`)

3. Restore and build:

```bash
dotnet restore
dotnet build
```

4. Create/apply EF migrations (from repo root):

```bash
# add migration
dotnet ef migrations add InitialCreate --project Ligot.DbApi --startup-project Ligot.AppHost --context ProjectDbContext
# apply migration
dotnet ef database update --project Ligot.DbApi --startup-project Ligot.AppHost --context ProjectDbContext
```

5. Run the application (AppHost):

```bash
dotnet run --project Ligot.AppHost
```

Visit the frontend or health endpoints as configured by your environment.

> Note: Connection strings are configured in `Ligot.DbApi/appsettings.json` and may be overridden via user secrets or environment variables in `Ligot.AppHost`.

---

## ✅ Running tests

Run unit and integration tests with:

```bash
dotnet test
```

Test projects live in `Ligot.Tests`.

---

## 📦 CI / CD

This repository includes GitHub Actions workflows for infrastructure deployment and application releases. See `QUICK-START.md` and `DEPLOYMENT.md` for full details and Azure setup instructions (including automated or manual service principal/GitHub secrets setup).

- Workflow files: `.github/workflows/` (example: `deploy-infrastructure.yml`)
- Quick setup: `QUICK-START.md`

---

## 🧭 Useful docs in this repo

- `QUICK-START.md` — Fast path for Azure CI/CD setup and first deployment
- `DEPLOYMENT.md` — Detailed deployment and operations guidance
- `Ligot.DbApi/README.md` — Database-specific dev notes and EF steps
- `docker-compose.postgres.yml` — Local Postgres + Adminer for development

---

## 🤝 Contributing

- Open issues for bugs or feature requests
- Follow repository contribution guidelines and maintainer reviews for PRs

---

## 📄 License

See `LICENSE.txt` in the repository root for license details.

---

If you want, I can add badges (build / test / license), or expand the README with architecture diagrams and contributor guidelines. 💡

