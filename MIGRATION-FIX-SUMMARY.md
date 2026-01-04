# Migration Fix - Follow-up

## Second Issue Discovered

After fixing the `init-db.sql` file, a second issue was found: **Entity Framework migrations themselves had PascalCase column names**, even though the DbContext was configured for snake_case.

### The Problem

The original migrations were generated with code like:
```csharp
Id = table.Column<int>(...),
Name = table.Column<string>(...),
ContactPerson = table.Column<string>(...)
```

This caused EF to generate SQL with quoted identifiers:
```sql
CREATE TABLE customers (
    "Id" integer,
    "Name" character varying(200),
    "ContactPerson" character varying(200),
    ...
)
```

But the DbContext configuration expected:
```csharp
b.Property(x => x.Id).HasColumnName("id");
b.Property(x => x.Name).HasColumnName("name");
b.Property(x => x.ContactPerson).HasColumnName("contact_person");
```

### The Solution

1. **Deleted all existing migrations** (5 migration files from Dec 30 - Jan 3)
2. **Fixed missing column mappings** in DbContext:
   - Added `IsActive` mapping for `User` entity
   - Added `UpdatedAt` mapping for `User` and `Role` entities
3. **Generated a single new migration** that properly uses snake_case column names
4. **Dropped and recreated the database** with the new migration
5. **Added sample data** (6 activities for Jan 2-4, 2026)

### Result

The new migration now generates correct SQL:
```csharp
id = table.Column<int>(...),
name = table.Column<string>(...),
contact_person = table.Column<string>(...)
```

Which produces unquoted lowercase identifiers:
```sql
CREATE TABLE customers (
    id integer,
    name character varying(200),
    contact_person character varying(200),
    ...
)
```

## Testing

The Aspire app is now running at `https://localhost:17252`.

Visit the home page and you should see:
- **January 2**: Initial client meeting
- **January 3**: Wireframe review, Kickoff meeting  
- **January 4**: Architecture planning, Content strategy session, Database assessment

## Files Modified

1. **ProjectDbContext.cs** - Added missing column mappings
2. **Migrations/** - Deleted old migrations, created new InitialCreate
3. **add-sample-data.ps1** - New script to add test data

## Key Learnings

1. **Column name consistency is critical** in PostgreSQL with EF Core
2. **Always configure column names explicitly** in DbContext when using non-standard naming
3. **Migrations inherit the configuration** at generation time - regenerate if config changes
4. **init-db.sql and migrations should not coexist** - choose one approach and stick with it

## Going Forward

- ✅ Use EF migrations exclusively for schema changes
- ✅ Remove or archive `init-db.sql` (now obsolete)
- ✅ Always verify column mappings when adding new entities
- ✅ Use `add-sample-data.ps1` for test data instead of init-db.sql
