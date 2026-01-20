-- Grant all roles to tomizawa user
-- This script ensures the tomizawa user has access to all areas of the application

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

-- Verify the setup
SELECT 
    u."WindowsUsername",
    u."DisplayName",
    u."IsActive",
    string_agg(r."Name", ', ' ORDER BY r."Name") as "RoleNames",
    string_agg(r."PagePermissions", ', ' ORDER BY r."Name") as "PagePermissions"
FROM "Users" u
JOIN "UserRoles" ur ON u."Id" = ur."UserId"
JOIN "Roles" r ON ur."RoleId" = r."Id"
WHERE u."WindowsUsername" = 'tomizawa'
GROUP BY u."Id", u."WindowsUsername", u."DisplayName", u."IsActive";
