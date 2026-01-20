# Script to grant all roles to tomizawa user
# Uses .NET and Npgsql to connect to PostgreSQL

param(
    [string]$DbHost = "localhost",
    [int]$Port = 5432,
    [string]$Database = "aspire_db",
    [string]$Username = "postgres",
    [string]$Password = "postgrespw"
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$csFile = Join-Path $scriptDir "UpdateUserRoles.cs"

Write-Host "Granting all roles to tomizawa user..."
Write-Host "Database: $DbHost`:$Port/$Database"

# Use dotnet script to execute the C# file
try {
    # Create a temporary .csx file that can be run with dotnet script
    $csharpScript = @"
#r "nuget: Npgsql, 7.0.0"

using Npgsql;

const string connectionString = "Host=$DbHost;Port=$Port;Database=$Database;Username=$Username;Password=$Password";

Console.WriteLine("Connecting to PostgreSQL database...");

try
{
    using (var connection = new NpgsqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("✓ Connected to database");

        // Read and execute the SQL script
        string sqlScript = System.IO.File.ReadAllText(@"$($csFile -replace '\.cs$', '.sql')");
        
        using (var cmd = new NpgsqlCommand(sqlScript, connection))
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("✓ Successfully granted all roles to tomizawa user!");
        }

        // Verify the setup
        string verifyQuery = @"
            SELECT 
                u.""WindowsUsername"",
                u.""DisplayName"",
                u.""IsActive"",
                string_agg(r.""Name"", ', ' ORDER BY r.""Name"") as ""RoleNames""
            FROM ""Users"" u
            JOIN ""UserRoles"" ur ON u.""Id"" = ur.""UserId""
            JOIN ""Roles"" r ON ur.""RoleId"" = r.""Id""
            WHERE u.""WindowsUsername"" = 'tomizawa'
            GROUP BY u.""Id"", u.""WindowsUsername"", u.""DisplayName"", u.""IsActive""";

        using (var cmd = new NpgsqlCommand(verifyQuery, connection))
        using (var reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                Console.WriteLine(@"`n✓ User: {0}"", reader["WindowsUsername"]);
                Console.WriteLine(@"  Display Name: {0}"", reader["DisplayName"]);
                Console.WriteLine(@"  Active: {0}"", reader["IsActive"]);
                Console.WriteLine(@"  Roles: {0}"", reader["RoleNames"]);
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(@"✗ Error: {0}"", ex.Message);
    Environment.Exit(1);
}
"@
    
    $scriptPath = Join-Path $scriptDir "update-roles.csx"
    $csharpScript | Set-Content $scriptPath -Encoding UTF8
    
    dotnet script $scriptPath
    
    Remove-Item $scriptPath -Force
    Write-Host "✓ Roles granted successfully!" -ForegroundColor Green
}
catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
    exit 1
}
