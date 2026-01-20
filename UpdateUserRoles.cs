using Npgsql;

// Database connection details
const string connectionString = "Host=localhost;Port=5432;Database=aspire_db;Username=postgres;Password=postgrespw";

Console.WriteLine("Connecting to PostgreSQL database...");

try
{
    using (var connection = new NpgsqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("✓ Connected to database");

        // Read and execute the SQL script
        string sqlScript = File.ReadAllText("grant-all-roles.sql");
        
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
                Console.WriteLine($"\n✓ User: {reader["WindowsUsername"]}");
                Console.WriteLine($"  Display Name: {reader["DisplayName"]}");
                Console.WriteLine($"  Active: {reader["IsActive"]}");
                Console.WriteLine($"  Roles: {reader["RoleNames"]}");
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
    Environment.Exit(1);
}
