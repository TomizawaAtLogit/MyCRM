# PowerShell script to grant all roles to tomizawa user using .NET
# This uses the Npgsql NuGet package directly

$DbHost = "localhost"
$Port = 5432
$Database = "aspire_db"
$Username = "postgres"
$Password = "postgrespw"

$connectionString = "Host=$DbHost;Port=$Port;Database=$Database;Username=$Username;Password=$Password"

# SQL script to execute
$sqlScript = @"
-- Grant all roles to tomizawa user

-- First, ensure the tomizawa user exists (if not already in database)
INSERT INTO "Users" ("WindowsUsername", "DisplayName", "Email", "IsActive")
VALUES ('tomizawa', 'Tomizawa', 'tomizawa@example.com', true)
ON CONFLICT ("WindowsUsername") DO NOTHING;

-- Get the user ID for tomizawa
WITH user_ids AS (
    SELECT "Id" FROM "Users" WHERE "WindowsUsername" = 'tomizawa'
)
-- Assign ALL roles to tomizawa user
INSERT INTO "UserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id"
FROM "Users" u
CROSS JOIN "Roles" r
WHERE u."WindowsUsername" = 'tomizawa'
ON CONFLICT DO NOTHING;
"@

Write-Host "Attempting to connect to PostgreSQL database..."
Write-Host "Host: $DbHost`:$Port, Database: $Database"

try {
    # Load assemblies
    Add-Type -AssemblyName System.Data
    
    # Try to load Npgsql via script package
    $NuGetDir = "$HOME\.nuget\packages\npgsql"
    if (Test-Path $NuGetDir) {
        $NpgsqlDll = Get-ChildItem -Path $NuGetDir -Recurse -Filter "Npgsql.dll" | Select-Object -First 1
        if ($NpgsqlDll) {
            Add-Type -Path $NpgsqlDll.FullName
            Write-Host "✓ Loaded Npgsql from: $($NpgsqlDll.FullName)"
        }
    } else {
        Write-Host "! Npgsql not found in NuGet cache, attempting alternative method..."
        # Try using .NET assembly loader
        $assembly = [System.Reflection.Assembly]::Load("Npgsql")
        Write-Host "✓ Loaded Npgsql assembly"
    }
    
    # Create connection
    $connection = New-Object Npgsql.NpgsqlConnection($connectionString)
    $connection.Open()
    Write-Host "✓ Connected to PostgreSQL database"
    
    # Execute SQL script
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    $command.ExecuteNonQuery()
    Write-Host "✓ Successfully granted all roles to tomizawa user!"
    
    # Verify the results
    $verifyQuery = @"
SELECT 
    u."WindowsUsername",
    u."DisplayName",
    u."IsActive",
    string_agg(r."Name", ', ' ORDER BY r."Name") as "RoleNames"
FROM "Users" u
JOIN "UserRoles" ur ON u."Id" = ur."UserId"
JOIN "Roles" r ON ur."RoleId" = r."Id"
WHERE u."WindowsUsername" = 'tomizawa'
GROUP BY u."Id", u."WindowsUsername", u."DisplayName", u."IsActive"
"@

    $verifyCommand = $connection.CreateCommand()
    $verifyCommand.CommandText = $verifyQuery
    $reader = $verifyCommand.ExecuteReader()
    
    if ($reader.Read()) {
        Write-Host "`n✓ User Configuration:"
        Write-Host "  Username: $($reader['WindowsUsername'])"
        Write-Host "  Display Name: $($reader['DisplayName'])"
        Write-Host "  Active: $($reader['IsActive'])"
        Write-Host "  Roles: $($reader['RoleNames'])"
    }
    
    $reader.Close()
    $connection.Close()
    Write-Host "`n✓ All roles have been granted successfully!"
    
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
    Write-Host "Make sure PostgreSQL is running and accessible at $DbHost`:$Port"
    exit 1
}
