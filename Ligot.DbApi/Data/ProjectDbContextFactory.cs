using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ligot.DbApi.Data
{
    // Design-time factory to avoid loading the AppHost during EF commands.
    public class ProjectDbContextFactory : IDesignTimeDbContextFactory<ProjectDbContext>
    {
        public ProjectDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ProjectDbContext>();

            // Prefer environment override, fall back to a sensible default for local development.
            var conn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? "Host=localhost;Port=5432;Database=aspire_db;Username=postgres;Password=postgrespw";

            builder.UseNpgsql(conn);

            return new ProjectDbContext(builder.Options);
        }
    }
}

