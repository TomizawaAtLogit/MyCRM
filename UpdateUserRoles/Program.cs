using System;
using System.Linq;
using System.Threading.Tasks;
using Ligot.DbApi;
using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = "Host=localhost;Port=5432;Database=aspire_db;Username=postgres;Password=postgrespw;";

        Console.WriteLine("Connecting to database...");

        try {
            // Create DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<ProjectDbContext>();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            optionsBuilder.UseNpgsql(connectionString);
            
            using (var context = new ProjectDbContext(optionsBuilder.Options))
            {
                Console.WriteLine("✓ Connected to database");

                // Get or create tomizawa user
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.WindowsUsername == "tomizawa");
                
                if (user == null) {
                    user = new User {
                        WindowsUsername = "tomizawa",
                        DisplayName = "Tomizawa",
                        Email = "tomizawa@example.com",
                        IsActive = true
                    };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                    Console.WriteLine("✓ Created user: tomizawa");
                } else {
                    Console.WriteLine("✓ Found existing user: tomizawa");
                }

                // Get all roles
                var roles = await context.Roles.ToListAsync();
                Console.WriteLine($"✓ Found {roles.Count} roles");

                // Update Admin role with all permissions
                var adminRole = roles.FirstOrDefault(r => r.Name == "Admin");
                if (adminRole != null)
                {
                    var expectedPermissions = "Admin,Support,PreSales,Cases,CaseTemplates,Projects,Customers,Audit,SlaConfiguration,Orders";
                    if (adminRole.PagePermissions != expectedPermissions)
                    {
                        Console.WriteLine($"  ! Updating Admin role permissions");
                        Console.WriteLine($"    From: {adminRole.PagePermissions}");
                        Console.WriteLine($"    To:   {expectedPermissions}");
                        adminRole.PagePermissions = expectedPermissions;
                        context.Roles.Update(adminRole);
                    }
                }

                // Assign all roles to the user
                var currentRoles = await context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                foreach (var role in roles) {
                    if (!currentRoles.Contains(role.Id)) {
                        var userRole = new UserRole {
                            UserId = user.Id,
                            RoleId = role.Id
                        };
                        context.UserRoles.Add(userRole);
                        Console.WriteLine($"  ✓ Assigned role: {role.Name}");
                    } else {
                        Console.WriteLine($"  → Role already assigned: {role.Name}");
                    }
                }

                await context.SaveChangesAsync();

                // Verify
                var verifyUser = await context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (verifyUser != null) {
                    Console.WriteLine("\n✓ User Configuration:");
                    Console.WriteLine($"  Username: {verifyUser.WindowsUsername}");
                    Console.WriteLine($"  Display Name: {verifyUser.DisplayName}");
                    Console.WriteLine($"  Active: {verifyUser.IsActive}");
                    Console.WriteLine($"  Assigned Roles:");
                    foreach (var ur in verifyUser.UserRoles) {
                        Console.WriteLine($"    • {ur.Role.Name}");
                        Console.WriteLine($"      Permissions: {ur.Role.PagePermissions}");
                        var perms = ur.Role.PagePermissions.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
                        Console.WriteLine($"      Individual: {string.Join(", ", perms)}");
                    }
                }

                Console.WriteLine("\n✓ All roles have been assigned successfully!");
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"✗ Error: {ex.Message}");
            if (ex.InnerException != null) {
                Console.WriteLine($"  Inner: {ex.InnerException.Message}");
            }
            Environment.Exit(1);
        }
    }
}

