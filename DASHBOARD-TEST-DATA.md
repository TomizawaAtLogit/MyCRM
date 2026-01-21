# Dashboard Test Data Guide

## Overview
We've created comprehensive dummy data for testing the Dashboard page which displays the four PDCA phases:
- **Plan**: Pre-sales Pipeline
- **Do**: Case Status  
- **Check**: Case Priority
- **Act**: Project Status

## What Was Created

Two files have been created to make it easy to load and reload dashboard test data:

### 1. SQL Script: `add-dashboard-test-data.sql`
Contains all the SQL INSERT statements for:
- 15 Pre-Sales Proposals (distributed across all stages)
- 14 Cases (with various statuses and priorities)
- 7 Projects (in different states)
- 11 Project Activities

### 2. PowerShell Script: `add-dashboard-test-data.ps1`
Executes the SQL script and provides a summary of what was added.

## How to Use

### Quick Start
Run the PowerShell script from the repository root:
```powershell
cd c:\Users\tomizawa\source\repos\AspireApp1
.\add-dashboard-test-data.ps1
```

### Manual SQL Execution
If you prefer to run the SQL directly:
```powershell
Get-Content .\add-dashboard-test-data.sql | docker exec -i aspireapp1-postgres-1 psql -U postgres -d aspire_db
```

## Data Added

### Pre-Sales Proposals (Plan Phase) - 15 Total
**Distribution by Stage:**
- **Identification** (3): Initial inquiries
- **Qualification** (4): Requirement gathering phase
- **Proposal** (3): Proposal development stage
- **Negotiation** (2): Negotiation in progress
- **Won** (2): Closed won deals
- **Lost** (1): Closed lost deals

### Cases (Do Phase) - 14 Total
**By Status:**
- **Open** (5): Recent unresolved issues
  - 1 Critical, 2 High, 1 Medium, 1 Low priority
- **In Progress** (4): Active work items
  - 1 Critical, 2 High, 1 Medium priority
- **Resolved** (3): Fixed but not yet closed
  - 1 Critical, 1 High, 1 Medium priority
- **Closed** (2): Complete cases

**By Priority:**
- Critical: 2 cases
- High: 5 cases  
- Medium: 4 cases
- Low: 3 cases

### Projects (Act Phase) - 7 Total
**By Status:**
- **Active (Wip)** (3): Currently in development
- **Completed (Closed)** (2): Finished projects
- **On Hold (Pending)** (2): Future or suspended projects

### Project Activities - 11 Total
Activities distributed across the three active projects:
- Website Redesign Phase 2: 4 activities
- Mobile App Development: 4 activities
- API Gateway Implementation: 3 activities

## Customer Distribution
The test data uses existing customers in your database:
- Customer ID 1: NICE Corporation
- Customer ID 3: ログイット
- Customer ID 4: 興安計装
- Customer ID 5: カバレッジで何も表示しない

## Dashboard Charts Populated

After loading this data, your dashboard will show:

1. **Plan - Pre-sales Pipeline**: Bar chart showing proposals by stage
2. **Do - Case Status**: Pie chart showing case distribution by status
3. **Check - Case Priority**: Pie chart showing case distribution by priority
4. **Act - Project Status**: Bar chart showing projects by status

## Test Metrics Displayed

The dashboard summary cards will now show:
- **Total Cases**: 14 open cases
- **Resolution Rate**: Based on resolved cases
- **SLA Compliance**: Based on cases resolved within/outside SLA
- **Active Projects**: 3 active projects out of 7 total

## Viewing the Data

1. Start the application: `dotnet run` from `AspireApp1.AppHost`
2. Navigate to `/dashboard` route
3. You should see all four PDCA charts populated with data

## Refreshing/Re-running

You can run the script multiple times. It will add duplicate data each time, which is fine for testing. If you want to clean up, you would need to:

```sql
-- Delete previously added test data
DELETE FROM project_activities WHERE summary IN ('Project kickoff', 'Design finalization', ...);
DELETE FROM projects WHERE name IN ('Website Redesign Phase 2', 'Mobile App Development', ...);
DELETE FROM cases WHERE title IN ('Login timeout issues', 'Report export failing', ...);
DELETE FROM presales_proposals WHERE title IN ('E-commerce Platform Inquiry', ...);
```

## Notes

- All test data uses realistic timestamps relative to now (using `NOW()` and INTERVAL functions)
- Customer foreign keys are satisfied by using existing customer IDs
- Project activities use subqueries to find the correct project IDs dynamically
- The data is suitable for testing dashboard rendering, filtering, and calculations
