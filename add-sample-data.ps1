# Add sample data to the database for testing
$ErrorActionPreference = "Stop"

Write-Host "Adding sample data to database..." -ForegroundColor Cyan

$sampleData = @"
-- Insert sample customers
INSERT INTO customers (name, contact_person, email, phone) VALUES 
('Acme Corporation', 'John Doe', 'john@acme.com', '123-456-7890'),
('Tech Solutions Inc', 'Jane Smith', 'jane@techsolutions.com', '098-765-4321');

-- Insert sample projects
INSERT INTO projects (name, description, customer_id, status) VALUES 
('Website Redesign', 'Complete redesign of corporate website', 1, 'Wip'),
('Mobile App Development', 'Native mobile app for iOS and Android', 2, 'Wip'),
('Database Migration', 'Migrate legacy database to PostgreSQL', 1, 'Pending');

-- Insert sample activities for January 2026
INSERT INTO project_activities (project_id, activity_date, summary, description, activity_type, performed_by) VALUES 
(1, '2026-01-02 10:00:00+00', 'Initial client meeting', 'Discussed project requirements and timeline', 'Meeting', 'Admin'),
(1, '2026-01-03 14:30:00+00', 'Wireframe review', 'Reviewed initial wireframes with design team', 'Design', 'Admin'),
(2, '2026-01-03 09:00:00+00', 'Kickoff meeting', 'Project kickoff with development team', 'Meeting', 'Admin'),
(2, '2026-01-04 11:00:00+00', 'Architecture planning', 'Designed application architecture', 'Development', 'Admin'),
(1, '2026-01-04 15:00:00+00', 'Content strategy session', 'Planned content migration strategy', 'Planning', 'Admin'),
(3, '2026-01-04 10:00:00+00', 'Database assessment', 'Analyzed current database structure', 'Analysis', 'Admin');
"@

try {
    $sampleData | docker exec -i aspireapp1-postgres-1 psql -U postgres -d aspire_db
    
    Write-Host "`nSample data added successfully!" -ForegroundColor Green
    Write-Host "  - 2 customers" -ForegroundColor White
    Write-Host "  - 3 projects" -ForegroundColor White
    Write-Host "  - 6 project activities (Jan 2-4, 2026)" -ForegroundColor White
    Write-Host "`nRefresh your browser to see the activities on the calendar." -ForegroundColor Yellow
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
