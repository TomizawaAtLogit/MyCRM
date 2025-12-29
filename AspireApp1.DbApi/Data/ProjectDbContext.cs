using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<CustomerDatabase> CustomerDatabases { get; set; } = null!;
        public DbSet<CustomerSite> CustomerSites { get; set; } = null!;
        public DbSet<CustomerSystem> CustomerSystems { get; set; } = null!;
        public DbSet<CustomerOrder> CustomerOrders { get; set; } = null!;
        public DbSet<ProjectActivity> ProjectActivities { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;

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

            modelBuilder.Entity<Customer>(b =>
            {
                b.ToTable("customers");
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(200);
                b.Property(x => x.ContactPerson).HasMaxLength(200);
                b.Property(x => x.Email).HasMaxLength(200);
                b.Property(x => x.Phone).HasMaxLength(50);
                b.Property(x => x.Address).HasMaxLength(500);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                
                b.HasMany(x => x.Databases).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Sites).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Systems).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Orders).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.ProjectActivities).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<CustomerDatabase>(b =>
            {
                b.ToTable("customer_databases");
                b.HasKey(x => x.Id);
                b.Property(x => x.DatabaseName).IsRequired().HasMaxLength(200);
                b.Property(x => x.DatabaseType).HasMaxLength(100);
                b.Property(x => x.ServerName).HasMaxLength(200);
                b.Property(x => x.Port).HasMaxLength(10);
                b.Property(x => x.Version).HasMaxLength(50);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.HasIndex(x => x.CustomerId);
            });

            modelBuilder.Entity<CustomerSite>(b =>
            {
                b.ToTable("customer_sites");
                b.HasKey(x => x.Id);
                b.Property(x => x.SiteName).IsRequired().HasMaxLength(200);
                b.Property(x => x.Address).HasMaxLength(500);
                b.Property(x => x.City).HasMaxLength(100);
                b.Property(x => x.State).HasMaxLength(100);
                b.Property(x => x.ZipCode).HasMaxLength(20);
                b.Property(x => x.Country).HasMaxLength(100);
                b.Property(x => x.ContactPerson).HasMaxLength(200);
                b.Property(x => x.Phone).HasMaxLength(50);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.HasIndex(x => x.CustomerId);
            });

            modelBuilder.Entity<CustomerSystem>(b =>
            {
                b.ToTable("customer_systems");
                b.HasKey(x => x.Id);
                b.Property(x => x.SystemName).IsRequired().HasMaxLength(200);
                b.Property(x => x.ComponentType).HasMaxLength(100);
                b.Property(x => x.Manufacturer).HasMaxLength(200);
                b.Property(x => x.Model).HasMaxLength(200);
                b.Property(x => x.SerialNumber).HasMaxLength(200);
                b.Property(x => x.Location).HasMaxLength(200);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.HasIndex(x => x.CustomerId);
            });

            modelBuilder.Entity<CustomerOrder>(b =>
            {
                b.ToTable("customer_orders");
                b.HasKey(x => x.Id);
                b.Property(x => x.OrderNumber).IsRequired().HasMaxLength(100);
                b.Property(x => x.ContractType).IsRequired().HasMaxLength(100);
                b.Property(x => x.BillingFrequency).HasMaxLength(50);
                b.Property(x => x.Status).HasMaxLength(50);
                b.Property(x => x.ContractValue).HasPrecision(18, 2);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.HasIndex(x => x.CustomerId);
                b.HasIndex(x => x.OrderNumber);
            });

            modelBuilder.Entity<ProjectActivity>(b =>
            {
                b.ToTable("project_activities");
                b.HasKey(x => x.Id);
                b.Property(x => x.Summary).IsRequired().HasMaxLength(500);
                b.Property(x => x.Description).HasMaxLength(5000);
                b.Property(x => x.NextAction).HasMaxLength(1000);
                b.Property(x => x.ActivityType).HasMaxLength(100);
                b.Property(x => x.PerformedBy).HasMaxLength(200);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                
                b.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(x => x.ProjectId);
                b.HasIndex(x => x.CustomerId);
                b.HasIndex(x => x.ActivityDate);
            });

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("users");
                b.HasKey(x => x.Id);
                b.Property(x => x.WindowsUsername).IsRequired().HasMaxLength(200);
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);
                b.Property(x => x.Email).HasMaxLength(200);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.HasIndex(x => x.WindowsUsername).IsUnique();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.ToTable("roles");
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(100);
                b.Property(x => x.Description).HasMaxLength(500);
                b.Property(x => x.PagePermissions).IsRequired().HasMaxLength(1000);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                b.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.ToTable("user_roles");
                b.HasKey(x => x.Id);
                b.Property(x => x.AssignedAt).HasDefaultValueSql("now()");
                
                b.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();
            });
        }
    }
}
