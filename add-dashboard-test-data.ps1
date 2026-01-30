# Add Dashboard Test Data Script
# Loads comprehensive test data to populate dashboard charts
$ErrorActionPreference = "Stop"

Write-Host "Loading dashboard test data..." -ForegroundColor Cyan
Write-Host ""

# Read the SQL file
$sqlFile = "$PSScriptRoot/add-dashboard-test-data.sql"
if (-not (Test-Path $sqlFile)) {
    Write-Host "Error: File not found: $sqlFile" -ForegroundColor Red
    exit 1
}

$sqlContent = Get-Content $sqlFile -Raw

try {
    # Execute the SQL file
    $sqlContent | docker exec -i Ligot-postgres-1 psql -U postgres -d aspire_db
    
    Write-Host ""
    Write-Host "Dashboard test data added successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Data Summary:" -ForegroundColor Yellow
    Write-Host "  Pre-Sales Proposals: 15 proposals" -ForegroundColor White
    Write-Host "    - Identification: 3" -ForegroundColor Gray
    Write-Host "    - Qualification: 4" -ForegroundColor Gray
    Write-Host "    - Proposal: 3" -ForegroundColor Gray
    Write-Host "    - Negotiation: 2" -ForegroundColor Gray
    Write-Host "    - Won: 2" -ForegroundColor Gray
    Write-Host "    - Lost: 1" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Cases: 14 cases" -ForegroundColor White
    Write-Host "    - Open: 5 (includes Critical, High, Medium, Low priorities)" -ForegroundColor Gray
    Write-Host "    - In Progress: 4 (mix of priorities)" -ForegroundColor Gray
    Write-Host "    - Resolved: 3" -ForegroundColor Gray
    Write-Host "    - Closed: 2" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Projects: 7 projects" -ForegroundColor White
    Write-Host "    - Active (Wip): 3" -ForegroundColor Gray
    Write-Host "    - Completed (Closed): 2" -ForegroundColor Gray
    Write-Host "    - On Hold (Pending): 2" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Activities: 10 project activities" -ForegroundColor White
    Write-Host ""
    Write-Host "Refresh your browser dashboard to see the charts populated!" -ForegroundColor Yellow
    Write-Host ""
}
catch {
    Write-Host "Error executing SQL: $_" -ForegroundColor Red
    exit 1
}

