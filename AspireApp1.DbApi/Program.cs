using AspireApp1.DbApi;
using AspireApp1.DbApi.Authorization;
using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Repositories;
using AspireApp1.DbApi.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Configure Npgsql to handle DateTime correctly with PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<FileUploadOperationFilter>();
});

// Add Windows Authentication - DISABLED FOR LOCAL DEVELOPMENT
// builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//     .AddNegotiate();

// Add Authorization with custom policies - DISABLED FOR LOCAL DEVELOPMENT
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy =>
//     {
//         policy.RequireAuthenticatedUser();
//         policy.Requirements.Add(new AdminPolicyRequirement());
//     });
//     
//     options.AddPolicy("SupportOnly", policy =>
//     {
//         policy.RequireAuthenticatedUser();
//         policy.Requirements.Add(new SupportPolicyRequirement());
//     });
//     
//     options.AddPolicy("PreSalesOnly", policy =>
//     {
//         policy.RequireAuthenticatedUser();
//         policy.Requirements.Add(new PreSalesPolicyRequirement());
//     });
// });

// builder.Services.AddSingleton<IAuthorizationHandler, AdminPolicyHandler>();
// builder.Services.AddSingleton<IAuthorizationHandler, SupportPolicyHandler>();
// builder.Services.AddSingleton<IAuthorizationHandler, PreSalesPolicyHandler>();

builder.Services.AddDbContext<ProjectDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProjectActivityRepository, ProjectActivityRepository>();
builder.Services.AddScoped<ICaseRepository, CaseRepository>();
builder.Services.AddScoped<ICaseActivityRepository, CaseActivityRepository>();
builder.Services.AddScoped<ICaseRelationshipRepository, CaseRelationshipRepository>();
builder.Services.AddScoped<ISlaConfigurationRepository, SlaConfigurationRepository>();
builder.Services.AddScoped<ICaseTemplateRepository, CaseTemplateRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// Add background service for audit log cleanup
builder.Services.AddHostedService<AuditCleanupService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

// DISABLED FOR LOCAL DEVELOPMENT
// app.UseAuthentication();
// app.UseAuthorization();

// Map controllers and a friendly root endpoint.
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
app.MapDefaultEndpoints();

// Apply any pending EF Core migrations at startup (useful for local dev).
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
    
    // Apply migrations instead of recreating database to preserve data
    db.Database.Migrate();
    
    // Seed initial data if database is empty
    if (!db.Users.Any())
    {
        SeedData(db);
    }
}
catch (Exception ex)
{
    // swallow or log — during CI you may want to rethrow
    Console.Error.WriteLine($"Migration/Seed failed: {ex}");
}

void SeedData(ProjectDbContext db)
{
    Console.WriteLine("Seeding initial data...");
    
    // Create default roles
    var adminRole = new Role
    {
        Name = "Admin",
        Description = "Administrator role with full access",
        PagePermissions = "Admin,Support,PreSales,Cases,CaseTemplates,Projects,Customers,Audit,SlaConfiguration,Orders"
    };
    var supportRole = new Role
    {
        Name = "Support",
        Description = "Support team role for case management",
        PagePermissions = "Support,Cases,CaseTemplates,Customers,SlaConfiguration,Audit"
    };
    var preSalesRole = new Role
    {
        Name = "PreSales",
        Description = "Pre-sales team role for project management",
        PagePermissions = "PreSales,Projects,Customers"
    };
    var userRole = new Role
    {
        Name = "User",
        Description = "Standard user role",
        PagePermissions = "Customers"
    };
    
    db.Roles.AddRange(adminRole, supportRole, preSalesRole, userRole);
    db.SaveChanges();

    // Create a default admin user (you should change this in production!)
    var adminUser = new User
    {
        WindowsUsername = Environment.UserName, // Current Windows user
        DisplayName = "Administrator",
        Email = "admin@example.com",
        IsActive = true
    };
    
    db.Users.Add(adminUser);
    db.SaveChanges();

    // Assign admin role to the default user
    var userRole1 = new UserRole
    {
        UserId = adminUser.Id,
        RoleId = adminRole.Id
    };
    
    db.UserRoles.Add(userRole1);
    db.SaveChanges();
    
    // Seed default SLA configurations
    var slaConfigs = new[]
    {
        new SlaThreshold { Priority = CasePriority.Critical, ResponseTimeHours = 1, ResolutionTimeHours = 4, IsActive = true },
        new SlaThreshold { Priority = CasePriority.High, ResponseTimeHours = 2, ResolutionTimeHours = 8, IsActive = true },
        new SlaThreshold { Priority = CasePriority.Medium, ResponseTimeHours = 4, ResolutionTimeHours = 24, IsActive = true },
        new SlaThreshold { Priority = CasePriority.Low, ResponseTimeHours = 8, ResolutionTimeHours = 48, IsActive = true }
    };
    
    db.SlaThresholds.AddRange(slaConfigs);
    db.SaveChanges();

    Console.WriteLine($"Seed data created. Default admin user: {adminUser.WindowsUsername}");
}

app.Run();