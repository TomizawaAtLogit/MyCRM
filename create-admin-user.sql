-- Create an Admin role
INSERT INTO "Roles" ("Name", "Description", "PagePermissions")
VALUES ('Admin', 'System Administrator', 'Admin,Projects,Customers,Orders')
ON CONFLICT DO NOTHING;

-- Create an admin user (replace 'tomizawa' with your actual Windows username)
INSERT INTO "Users" ("WindowsUsername", "DisplayName", "Email", "IsActive")
VALUES ('tomizawa', '富澤', 'tomizawa@logit.co.jp', true)
ON CONFLICT ("WindowsUsername") DO NOTHING;

-- Assign Admin role to the user
INSERT INTO "UserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id"
FROM "Users" u
CROSS JOIN "Roles" r
WHERE u."WindowsUsername" = 'tomizawa' 
  AND r."Name" = 'Admin'
ON CONFLICT DO NOTHING;

-- Verify the setup
SELECT 
    u."WindowsUsername",
    u."DisplayName",
    u."IsActive",
    r."Name" as "RoleName",
    r."PagePermissions"
FROM "Users" u
JOIN "UserRoles" ur ON u."Id" = ur."UserId"
JOIN "Roles" r ON ur."RoleId" = r."Id"
WHERE u."WindowsUsername" = 'tomizawa';
