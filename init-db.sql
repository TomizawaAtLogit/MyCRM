-- Create projects table
CREATE TABLE IF NOT EXISTS projects (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Insert sample data
INSERT INTO projects ("Name", "Description", "CreatedAt")
VALUES 
    ('Sample Project 1', 'This is a test project', NOW()),
    ('Sample Project 2', 'Another test project', NOW())
ON CONFLICT DO NOTHING;
