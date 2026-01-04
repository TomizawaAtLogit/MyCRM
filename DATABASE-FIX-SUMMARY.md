# Database Schema Fix - Summary

## Problem Identified

The calendar was showing "No activities" because of a **database schema mismatch** between the `init-db.sql` file and Entity Framework migrations.

### Root Cause

1. **Column Name Mismatch**: The `init-db.sql` file used **PascalCase with quotes** (e.g., `"Id"`, `"ProjectId"`, `"CustomerId"`), while Entity Framework migrations use **snake_case** without quotes (e.g., `id`, `project_id`, `customer_id`).

2. **Schema Evolution**: The `project_activities` table in `init-db.sql` had a `"CustomerId"` column that was removed in migration `20251231075641_MoveCustomerIdToProjects.cs`. This column was moved to the `projects` table instead.

3. **Missing Columns**: The `init-db.sql` was outdated and missing several columns and tables that were added through migrations.

### Impact

When the backend tried to query the database, it expected columns like `project_id`, `activity_date`, etc., but the actual database had columns like `"ProjectId"`, `"ActivityDate"`, etc. This caused SQL execution errors, preventing activities from being loaded.

## Solution Applied

### 1. Updated init-db.sql

- ✅ Changed all column names from **PascalCase** to **snake_case**
- ✅ Removed `customer_id` from `project_activities` table
- ✅ Added `customer_id` and `status` columns to `projects` table
- ✅ Added missing `updated_at` columns to tables
- ✅ Added missing tables: `systems`, `system_components`, `audit_logs`, `entity_files`
- ✅ Removed quoted identifiers (PostgreSQL now uses lowercase)
- ✅ Reordered tables to respect foreign key dependencies

### 2. Created recreate-db.ps1 Script

A PowerShell script to:
- Drop and recreate the PostgreSQL database
- Run the corrected `init-db.sql`
- Add sample data including 6 project activities for January 2026

### 3. Database Recreation

Successfully recreated the database with:
- 2 sample customers
- 3 sample projects
- 6 project activities (Jan 2-4, 2026)
- 1 admin user with proper roles

## Verification

The Aspire application is now running at `https://localhost:17252`.

To verify the fix:
1. Navigate to the home page (Activity Calendar)
2. You should now see activities displayed on the calendar for:
   - January 2: Initial client meeting
   - January 3: Wireframe review, Kickoff meeting
   - January 4: Architecture planning, Content strategy session, Database assessment

## Files Modified

1. **init-db.sql** - Complete rewrite to match EF schema
2. **recreate-db.ps1** - New script to rebuild database with Docker

## Prevention

Going forward:
- Always use **Entity Framework migrations** for schema changes
- Keep `init-db.sql` in sync with migrations, or remove it and rely solely on migrations
- Use `dotnet ef migrations script` to generate SQL from migrations if needed

## Notes

The schema mismatch occurred because:
- The database was initially created using `init-db.sql` (with PascalCase)
- Later migrations changed the schema (snake_case, moved customer_id)
- The `init-db.sql` file was never updated to reflect these changes
- Running the app with migrations would partially fix the schema, but there were fundamental incompatibilities
