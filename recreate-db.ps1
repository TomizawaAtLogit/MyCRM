# Script to recreate the database with the correct schema using Docker
# This will drop the existing database and create a new one using init-db.sql

$ErrorActionPreference = "Stop"

Write-Host "Stopping Aspire app if running..." -ForegroundColor Yellow
# Stop any running Aspire processes
Get-Process | Where-Object { $_.ProcessName -like "*AspireApp1*" -or $_.ProcessName -eq "dotnet" } | Where-Object { $_.Path -like "*AspireApp1*" } | Stop-Process -Force -ErrorAction SilentlyContinue

Write-Host "Recreating PostgreSQL database using Docker..." -ForegroundColor Cyan

# Database configuration (from docker-compose.postgres.yml)
$dbName = "aspire_db"
$dbUser = "postgres"
$dbPassword = "postgrespw"
$containerName = "aspireapp1-postgres-1"

try {
    # Check if Docker container is running
    $containerRunning = docker ps --filter "name=$containerName" --format "{{.Names}}" 2>$null
    if (-not $containerRunning) {
        Write-Host "PostgreSQL container not running. Starting it..." -ForegroundColor Yellow
        docker-compose -f docker-compose.postgres.yml up -d
        Start-Sleep -Seconds 5
    }
    
    # Drop and recreate the database
    Write-Host "Dropping existing database '$dbName'..." -ForegroundColor Yellow
    docker exec -i $containerName psql -U $dbUser -c "DROP DATABASE IF EXISTS $dbName;" 2>$null
    
    Write-Host "Creating new database '$dbName'..." -ForegroundColor Green
    docker exec -i $containerName psql -U $dbUser -c "CREATE DATABASE $dbName;"
    
    # Run the init-db.sql script
    Write-Host "Running init-db.sql..." -ForegroundColor Green
    Get-Content "init-db.sql" | docker exec -i $containerName psql -U $dbUser -d $dbName
    
    # Add sample data
    Write-Host "Adding sample data..." -ForegroundColor Green
    $sampleData = @"
-- Insert sample customers
INSERT INTO customers (name, contact_person, email, phone) VALUES 
('Acme Corporation', 'John Doe', 'john@acme.com', '123-456-7890'),
('Tech Solutions Inc', 'Jane Smith', 'jane@techsolutions.com', '098-765-4321');

-- Insert sample projects
INSERT INTO projects (name, description, customer_id, status) VALUES 
('Website Redesign', 'Complete redesign of corporate website', 1, 'Wip'),
('Mobile App Development', 'Native mobile app for iOS and Android', 2, 'Wip'),
('Database Migration', 'Migrate legacy database to PostgreSQL', 1, 'Planning');

-- Insert sample activities for January 2026
INSERT INTO project_activities (project_id, activity_date, summary, description, activity_type, performed_by) VALUES 
(1, '2026-01-02 10:00:00', 'Initial client meeting', 'Discussed project requirements and timeline', 'Meeting', 'Admin'),
(1, '2026-01-03 14:30:00', 'Wireframe review', 'Reviewed initial wireframes with design team', 'Design', 'Admin'),
(2, '2026-01-03 09:00:00', 'Kickoff meeting', 'Project kickoff with development team', 'Meeting', 'Admin'),
(2, '2026-01-04 11:00:00', 'Architecture planning', 'Designed application architecture', 'Development', 'Admin'),
(1, '2026-01-04 15:00:00', 'Content strategy session', 'Planned content migration strategy', 'Planning', 'Admin'),
(3, '2026-01-04 10:00:00', 'Database assessment', 'Analyzed current database structure', 'Analysis', 'Admin');

-- Insert admin user
INSERT INTO users (windows_username, display_name, email, is_active) VALUES 
('$env:USERNAME', 'Administrator', 'admin@example.com', true);

-- Insert roles
INSERT INTO roles (name, description, page_permissions) VALUES 
('Admin', 'Administrator with full access', 'Admin,Projects,Customers,Orders,Audit'),
('User', 'Standard user', 'Projects,Customers');

-- Assign admin role to user
INSERT INTO user_roles (user_id, role_id) VALUES (1, 1);
"@
    
    $sampleData | docker exec -i $containerName psql -U $dbUser -d $dbName
    
    Write-Host "`nDatabase recreated successfully!" -ForegroundColor Green
    Write-Host "Sample data added:" -ForegroundColor Cyan
    Write-Host "  - 2 customers" -ForegroundColor White
    Write-Host "  - 3 projects" -ForegroundColor White
    Write-Host "  - 6 project activities (for Jan 2-4, 2026)" -ForegroundColor White
    Write-Host "  - 1 admin user" -ForegroundColor White
    
    Write-Host "`nYou can now start the Aspire app:" -ForegroundColor Yellow
    Write-Host "  cd AspireApp1.AppHost" -ForegroundColor White
    Write-Host "  dotnet run" -ForegroundColor White
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host "Make sure Docker is running and the PostgreSQL container is available." -ForegroundColor Yellow
    exit 1
}
