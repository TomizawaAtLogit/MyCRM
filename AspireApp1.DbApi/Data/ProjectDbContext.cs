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
        public DbSet<Models.System> Systems { get; set; } = null!;
        public DbSet<SystemComponent> SystemComponents { get; set; } = null!;
        public DbSet<CustomerOrder> CustomerOrders { get; set; } = null!;
        public DbSet<ProjectActivity> ProjectActivities { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<EntityFile> EntityFiles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use lowercase table name to match PostgreSQL default identifier folding.
            modelBuilder.Entity<Project>(b =>
            {
                b.ToTable("projects");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).IsRequired().HasColumnName("name");
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.CustomerId).HasColumnName("customer_id");
                b.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                
                b.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(x => x.CustomerId);
            });

            modelBuilder.Entity<Customer>(b =>
            {
                b.ToTable("customers");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).IsRequired().HasMaxLength(200).HasColumnName("name");
                b.Property(x => x.ContactPerson).HasMaxLength(200).HasColumnName("contact_person");
                b.Property(x => x.Email).HasMaxLength(200).HasColumnName("email");
                b.Property(x => x.Phone).HasMaxLength(50).HasColumnName("phone");
                b.Property(x => x.Address).HasMaxLength(500).HasColumnName("address");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                
                b.HasMany(x => x.Databases).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Sites).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.CustomerSystems).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Systems).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Orders).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CustomerDatabase>(b =>
            {
                b.ToTable("customer_databases");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.CustomerId).HasColumnName("customer_id");
                b.Property(x => x.DatabaseName).IsRequired().HasMaxLength(200).HasColumnName("database_name");
                b.Property(x => x.DatabaseType).HasMaxLength(100).HasColumnName("database_type");
                b.Property(x => x.ServerName).HasMaxLength(200).HasColumnName("server_name");
                b.Property(x => x.Port).HasMaxLength(10).HasColumnName("port");
                b.Property(x => x.Version).HasMaxLength(50).HasColumnName("version");
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.HasIndex(x => x.CustomerId);
            });

            modelBuilder.Entity<CustomerSite>(b =>
            {
                b.ToTable("customer_sites");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.CustomerId).HasColumnName("customer_id");
                b.Property(x => x.SiteName).IsRequired().HasMaxLength(200).HasColumnName("site_name");
                b.Property(x => x.Address).HasMaxLength(500).HasColumnName("address");
                b.Property(x => x.PostCode).HasMaxLength(20).HasColumnName("post_code");
                b.Property(x => x.Country).HasMaxLength(100).HasColumnName("country");
                b.Property(x => x.ContactPerson).HasMaxLength(200).HasColumnName("contact_person");
                b.Property(x => x.Phone).HasMaxLength(50).HasColumnName("phone");
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.HasIndex(x => x.CustomerId);
            });

            modelBuilder.Entity<CustomerSystem>(b =>
            {
                b.ToTable("customer_systems");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.CustomerId).HasColumnName("customer_id");
                b.Property(x => x.SystemName).IsRequired().HasMaxLength(200).HasColumnName("system_name");
                b.Property(x => x.ComponentType).HasMaxLength(100).HasColumnName("component_type");
                b.Property(x => x.Manufacturer).HasMaxLength(200).HasColumnName("manufacturer");
                b.Property(x => x.Model).HasMaxLength(200).HasColumnName("model");
                b.Property(x => x.SerialNumber).HasMaxLength(200).HasColumnName("serial_number");
                b.Property(x => x.Location).HasMaxLength(200).HasColumnName("location");
                b.Property(x => x.InstallationDate).HasColumnName("installation_date");
                b.Property(x => x.WarrantyExpiration).HasColumnName("warranty_expiration");
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.HasIndex(x => x.CustomerId);
            });

            modelBuilder.Entity<Models.System>(b =>
            {
                b.ToTable("systems");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.CustomerId).HasColumnName("customer_id");
                b.Property(x => x.SystemName).IsRequired().HasMaxLength(200).HasColumnName("system_name");
                b.Property(x => x.InstallationDate).HasColumnName("installation_date");
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                
                b.HasOne(x => x.Customer).WithMany(x => x.Systems).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Components).WithOne(x => x.System).HasForeignKey(x => x.SystemId).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(x => x.CustomerId);
            });

            modelBuilder.Entity<SystemComponent>(b =>
            {
                b.ToTable("system_components");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.SystemId).HasColumnName("system_id");
                b.Property(x => x.ComponentType).IsRequired().HasMaxLength(100).HasColumnName("component_type");
                b.Property(x => x.Manufacturer).HasMaxLength(200).HasColumnName("manufacturer");
                b.Property(x => x.Model).HasMaxLength(200).HasColumnName("model");
                b.Property(x => x.SerialNumber).HasMaxLength(200).HasColumnName("serial_number");
                b.Property(x => x.Location).HasMaxLength(200).HasColumnName("location");
                b.Property(x => x.WarrantyExpiration).HasColumnName("warranty_expiration");
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.HasIndex(x => x.SystemId);
                b.HasIndex(x => x.SerialNumber);
            });

            modelBuilder.Entity<CustomerOrder>(b =>
            {
                b.ToTable("customer_orders");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.CustomerId).HasColumnName("customer_id");
                b.Property(x => x.OrderNumber).IsRequired().HasMaxLength(100).HasColumnName("order_number");
                b.Property(x => x.ContractType).IsRequired().HasMaxLength(100).HasColumnName("contract_type");
                b.Property(x => x.StartDate).HasColumnName("start_date");
                b.Property(x => x.EndDate).HasColumnName("end_date");
                b.Property(x => x.ContractValue).HasPrecision(18, 2).HasColumnName("contract_value");
                b.Property(x => x.BillingFrequency).HasMaxLength(50).HasColumnName("billing_frequency");
                b.Property(x => x.Status).HasMaxLength(50).HasColumnName("status");
                b.Property(x => x.Description).HasColumnName("description");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.HasIndex(x => x.CustomerId);
                b.HasIndex(x => x.OrderNumber);
            });

            modelBuilder.Entity<ProjectActivity>(b =>
            {
                b.ToTable("project_activities");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ProjectId).HasColumnName("project_id");
                b.Property(x => x.ActivityDate).HasColumnName("activity_date");
                b.Property(x => x.Summary).IsRequired().HasMaxLength(500).HasColumnName("summary");
                b.Property(x => x.Description).HasMaxLength(5000).HasColumnName("description");
                b.Property(x => x.NextAction).HasMaxLength(1000).HasColumnName("next_action");
                b.Property(x => x.ActivityType).HasMaxLength(100).HasColumnName("activity_type");
                b.Property(x => x.PerformedBy).HasMaxLength(200).HasColumnName("performed_by");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                
                b.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(x => x.ProjectId);
                b.HasIndex(x => x.ActivityDate);
            });

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("users");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.WindowsUsername).IsRequired().HasMaxLength(200).HasColumnName("windows_username");
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(200).HasColumnName("display_name");
                b.Property(x => x.Email).HasMaxLength(200).HasColumnName("email");
                b.Property(x => x.PreferredLanguage).HasMaxLength(10).HasColumnName("preferred_language");
                b.Property(x => x.IsActive).HasColumnName("is_active");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.HasIndex(x => x.WindowsUsername).IsUnique();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.ToTable("roles");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
                b.Property(x => x.Description).HasMaxLength(500).HasColumnName("description");
                b.Property(x => x.PagePermissions).IsRequired().HasMaxLength(1000).HasColumnName("page_permissions");
                b.Property(x => x.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                b.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.ToTable("user_roles");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.UserId).HasColumnName("user_id");
                b.Property(x => x.RoleId).HasColumnName("role_id");
                b.Property(x => x.AssignedAt).HasDefaultValueSql("now()").HasColumnName("assigned_at");
                
                b.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();
            });

            modelBuilder.Entity<AuditLog>(b =>
            {
                b.ToTable("audit_logs");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Timestamp).HasDefaultValueSql("now()").HasColumnName("timestamp");
                b.Property(x => x.UserId).HasColumnName("user_id");
                b.Property(x => x.Username).IsRequired().HasMaxLength(200).HasColumnName("username");
                b.Property(x => x.Action).IsRequired().HasMaxLength(50).HasColumnName("action");
                b.Property(x => x.EntityType).IsRequired().HasMaxLength(100).HasColumnName("entity_type");
                b.Property(x => x.EntityId).HasColumnName("entity_id");
                b.Property(x => x.EntitySnapshot).HasColumnName("entity_snapshot");
                b.Property(x => x.RetentionUntil).HasColumnName("retention_until");
                
                b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
                b.HasIndex(x => x.Timestamp);
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.EntityType);
                b.HasIndex(x => x.EntityId);
                b.HasIndex(x => x.RetentionUntil);
            });

            modelBuilder.Entity<EntityFile>(b =>
            {
                b.ToTable("entity_files");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.EntityType).IsRequired().HasMaxLength(50).HasColumnName("entity_type");
                b.Property(x => x.EntityId).HasColumnName("entity_id");
                b.Property(x => x.FileName).IsRequired().HasMaxLength(500).HasColumnName("file_name");
                b.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(500).HasColumnName("original_file_name");
                b.Property(x => x.StoragePath).IsRequired().HasMaxLength(1000).HasColumnName("storage_path");
                b.Property(x => x.FileSizeBytes).HasColumnName("file_size_bytes");
                b.Property(x => x.ContentType).IsRequired().HasMaxLength(200).HasColumnName("content_type");
                b.Property(x => x.Description).HasMaxLength(2000).HasColumnName("description");
                b.Property(x => x.Tags).HasMaxLength(2000).HasColumnName("tags");
                b.Property(x => x.ThumbnailPath).HasMaxLength(1000).HasColumnName("thumbnail_path");
                b.Property(x => x.UploadedAt).HasDefaultValueSql("now()").HasColumnName("uploaded_at");
                b.Property(x => x.UploadedBy).IsRequired().HasMaxLength(200).HasColumnName("uploaded_by");
                b.Property(x => x.LastAccessedAt).HasColumnName("last_accessed_at");
                b.Property(x => x.AccessCount).HasDefaultValue(0).HasColumnName("access_count");
                b.Property(x => x.RetentionUntil).HasColumnName("retention_until");
                b.Property(x => x.IsCompressed).HasDefaultValue(false).HasColumnName("is_compressed");
                b.Property(x => x.OriginalSizeBytes).HasColumnName("original_size_bytes");
                
                b.HasIndex(x => new { x.EntityType, x.EntityId });
                b.HasIndex(x => x.UploadedAt);
                b.HasIndex(x => x.RetentionUntil);
            });
        }
    }
}
