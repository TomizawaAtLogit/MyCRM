#!/usr/bin/env dotnet-script
#r "nuget: Npgsql, 8.0.2"

using Npgsql;

const string connectionString = "Host=localhost;Port=5432;Database=aspire_db;Username=postgres;Password=postgrespw";

try
{
    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();
    Console.WriteLine("✓ Connected to PostgreSQL");

    // SQL script
    var sqlScript = @"
-- Ensure tomizawa user exists
INSERT INTO ""Users"" (""WindowsUsername"", ""DisplayName"", ""Email"", ""IsActive"")
VALUES ('tomizawa', 'Tomizawa', 'tomizawa@example.com', true)
ON CONFLICT (""WindowsUsername"") DO NOTHING;

-- Assign all roles to tomizawa
INSERT INTO ""UserRoles"" (""UserId"", ""RoleId"")
SELECT u.""Id"", r.""Id""
FROM ""Users"" u
CROSS JOIN ""Roles"" r
WHERE u.""WindowsUsername"" = 'tomizawa'
ON CONFLICT DO NOTHING;
";

    using var cmd = new NpgsqlCommand(sqlScript, connection);
    cmd.ExecuteNonQuery();
    Console.WriteLine("✓ Assigned all roles to tomizawa");

    // Verify
    var verifyQuery = @"
SELECT 
    u.""WindowsUsername"",
    u.""DisplayName"",
    string_agg(r.""Name"", ', ' ORDER BY r.""Name"") as ""Roles""
FROM ""Users"" u
JOIN ""UserRoles"" ur ON u.""Id"" = ur.""UserId""
JOIN ""Roles"" r ON ur.""RoleId"" = r.""Id""
WHERE u.""WindowsUsername"" = 'tomizawa'
GROUP BY u.""Id"", u.""WindowsUsername"", u.""DisplayName""";

    using var verifyCmd = new NpgsqlCommand(verifyQuery, connection);
    using var reader = verifyCmd.ExecuteReader();
    if (reader.Read())
    {
        Console.WriteLine($"\n✓ User: {reader["WindowsUsername"]}");
        Console.WriteLine($"  Display Name: {reader["DisplayName"]}");
        Console.WriteLine($"  Roles: {reader["Roles"]}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
    Environment.Exit(1);
}
