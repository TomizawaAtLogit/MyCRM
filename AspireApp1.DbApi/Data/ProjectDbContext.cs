using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use lowercase table name to match PostgreSQL default identifier folding.
            modelBuilder.Entity<Project>(b =>
            {
                b.ToTable("projects");
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired();
                b.Property(x => x.Description);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            });
        }
    }
}
