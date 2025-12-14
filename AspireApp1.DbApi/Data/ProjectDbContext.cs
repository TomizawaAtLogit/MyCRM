using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; } = null!;
    }
}
