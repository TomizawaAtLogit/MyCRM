-- Create projects table
CREATE TABLE IF NOT EXISTS projects (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create users table
CREATE TABLE IF NOT EXISTS users (
    "Id" SERIAL PRIMARY KEY,
    "WindowsUsername" VARCHAR(200) NOT NULL UNIQUE,
    "DisplayName" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(200),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP
);

-- Create roles table
CREATE TABLE IF NOT EXISTS roles (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE,
    "Description" VARCHAR(500),
    "PagePermissions" VARCHAR(1000) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP
);

-- Create user_roles table
CREATE TABLE IF NOT EXISTS user_roles (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES users("Id") ON DELETE CASCADE,
    "RoleId" INTEGER NOT NULL REFERENCES roles("Id") ON DELETE CASCADE,
    "AssignedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    UNIQUE("UserId", "RoleId")
);

-- Create customers table
CREATE TABLE IF NOT EXISTS customers (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "ContactPerson" VARCHAR(200),
    "Email" VARCHAR(200),
    "Phone" VARCHAR(50),
    "Address" VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create customer_databases table
CREATE TABLE IF NOT EXISTS customer_databases (
    "Id" SERIAL PRIMARY KEY,
    "CustomerId" INTEGER NOT NULL REFERENCES customers("Id") ON DELETE CASCADE,
    "DatabaseName" VARCHAR(200) NOT NULL,
    "DatabaseType" VARCHAR(100),
    "ServerName" VARCHAR(200),
    "Port" VARCHAR(10),
    "Version" VARCHAR(50),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create customer_sites table
CREATE TABLE IF NOT EXISTS customer_sites (
    "Id" SERIAL PRIMARY KEY,
    "CustomerId" INTEGER NOT NULL REFERENCES customers("Id") ON DELETE CASCADE,
    "SiteName" VARCHAR(200) NOT NULL,
    "Address" VARCHAR(500),
    "City" VARCHAR(100),
    "State" VARCHAR(100),
    "ZipCode" VARCHAR(20),
    "Country" VARCHAR(100),
    "ContactPerson" VARCHAR(200),
    "Phone" VARCHAR(50),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create customer_systems table
CREATE TABLE IF NOT EXISTS customer_systems (
    "Id" SERIAL PRIMARY KEY,
    "CustomerId" INTEGER NOT NULL REFERENCES customers("Id") ON DELETE CASCADE,
    "SystemName" VARCHAR(200) NOT NULL,
    "ComponentType" VARCHAR(100),
    "Manufacturer" VARCHAR(200),
    "Model" VARCHAR(200),
    "SerialNumber" VARCHAR(200),
    "Location" VARCHAR(200),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create customer_orders table
CREATE TABLE IF NOT EXISTS customer_orders (
    "Id" SERIAL PRIMARY KEY,
    "CustomerId" INTEGER NOT NULL REFERENCES customers("Id") ON DELETE CASCADE,
    "OrderNumber" VARCHAR(100) NOT NULL UNIQUE,
    "ContractType" VARCHAR(100) NOT NULL,
    "BillingFrequency" VARCHAR(50),
    "Status" VARCHAR(50),
    "ContractValue" DECIMAL(18, 2),
    "ContractStartDate" DATE,
    "ContractEndDate" DATE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create project_activities table
CREATE TABLE IF NOT EXISTS project_activities (
    "Id" SERIAL PRIMARY KEY,
    "ProjectId" INTEGER NOT NULL REFERENCES projects("Id") ON DELETE CASCADE,
    "CustomerId" INTEGER REFERENCES customers("Id") ON DELETE SET NULL,
    "Summary" VARCHAR(500) NOT NULL,
    "Description" VARCHAR(5000),
    "NextAction" VARCHAR(1000),
    "ActivityType" VARCHAR(100),
    "PerformedBy" VARCHAR(200),
    "ActivityDate" TIMESTAMP NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create indexes
CREATE INDEX IF NOT EXISTS idx_customer_databases_customerid ON customer_databases("CustomerId");
CREATE INDEX IF NOT EXISTS idx_customer_sites_customerid ON customer_sites("CustomerId");
CREATE INDEX IF NOT EXISTS idx_customer_systems_customerid ON customer_systems("CustomerId");
CREATE INDEX IF NOT EXISTS idx_customer_orders_customerid ON customer_orders("CustomerId");
CREATE INDEX IF NOT EXISTS idx_project_activities_projectid ON project_activities("ProjectId");
CREATE INDEX IF NOT EXISTS idx_project_activities_customerid ON project_activities("CustomerId");
CREATE INDEX IF NOT EXISTS idx_project_activities_activitydate ON project_activities("ActivityDate");
