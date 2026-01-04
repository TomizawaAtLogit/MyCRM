# DateTime/Timezone Fix

## Third Issue: DateTime Kind Mismatch

After fixing the schema and migrations, a third issue appeared: **PostgreSQL timestamp handling**.

### The Error

```
Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone'
```

### Root Cause

PostgreSQL's `timestamp with time zone` type requires DateTime values to have `Kind=Utc` when using Npgsql (the .NET PostgreSQL provider). When creating DateTime values like:

```csharp
var startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
```

The resulting DateTime has `Kind=Unspecified`, which Npgsql rejects for `timestamp with time zone` columns.

### The Solution

Enable Npgsql's **legacy timestamp behavior** which allows DateTime values with any Kind to be written to PostgreSQL. This is added in `Program.cs`:

```csharp
// Configure Npgsql to handle DateTime correctly with PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```

This switch tells Npgsql to:
- Accept DateTime values regardless of their Kind property
- Treat all DateTime values as UTC when writing to `timestamp with time zone` columns
- Return all timestamps as local time when reading

### Alternative Solutions

If you want more explicit control, you could instead:

1. **Always use UTC explicitly:**
   ```csharp
   var startDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
   ```

2. **Convert to UTC before queries:**
   ```csharp
   var startDate = DateTime.SpecifyKind(new DateTime(...), DateTimeKind.Utc);
   ```

3. **Use DateTimeOffset instead of DateTime** in models

The `EnableLegacyTimestampBehavior` flag is the simplest solution for existing codebases.

## Testing

Refresh your browser at https://localhost:7030 and the calendar should now display the 6 activities for January 2-4, 2026.

## File Changed

- **AspireApp1.DbApi/Program.cs** - Added Npgsql.EnableLegacyTimestampBehavior switch
