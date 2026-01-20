# Coverage-Based Filtering

## Overview

This document describes the coverage-based filtering feature that applies across all pages in the MyCRM application. The filtering ensures that users only see data related to customers they have access to based on their role coverage assignments.

## How It Works

### Role Coverage Concept

- **RoleCoverage**: A mapping between roles and customers in the `role_coverages` table
- Each entry grants a specific role access to a specific customer

### Access Rules

1. **Unrestricted Access**: If a role has NO coverage entries (CoverageCount = 0), users with that role can access ALL customers
2. **Restricted Access**: If a role has coverage entries, users with that role can ONLY access those specific customers
3. **Multiple Roles**: Users with multiple roles get the UNION of all their roles' coverage
4. **No Roles**: Users without any roles have NO access to any customers

### Example Scenarios

#### Scenario 1: Unrestricted Role
```
User: admin
Role: Administrator (no coverage entries)
Result: Can see ALL customers and related data
```

#### Scenario 2: Restricted Role
```
User: tomizawa
Role: Sales (coverage for customers: ログイット, ABC Corp)
Result: Can ONLY see ログイット and ABC Corp and their related data
```

#### Scenario 3: Multiple Roles
```
User: manager
Roles: 
  - Sales (coverage for: ログイット, ABC Corp)
  - Support (coverage for: XYZ Ltd)
Result: Can see ログイット, ABC Corp, AND XYZ Ltd (union of all)
```

#### Scenario 4: Mixed Roles
```
User: supervisor
Roles:
  - Manager (no coverage entries - unrestricted)
  - Support (coverage for: ログイット)
Result: Can see ALL customers (one unrestricted role grants full access)
```

## Implementation Details

### Database Layer

#### UserRepository
```csharp
Task<int[]?> GetAllowedCustomerIdsAsync(int userId)
```
- Returns `null`: User has unrestricted access (at least one role with no coverage)
- Returns `int[]`: Array of allowed customer IDs
- Returns empty `int[]`: User has no roles or no access

### Repository Methods

Each repository that deals with customer-related data has an overloaded method:
```csharp
Task<IEnumerable<T>> GetAllAsync(int[]? allowedCustomerIds)
```

Repositories with coverage filtering:
- CustomerRepository
- ProjectRepository
- CaseRepository
- PreSalesActivityRepository
- PreSalesProposalRepository
- RequirementDefinitionRepository
- OrderRepository

### Controller Layer

All GET endpoints follow this pattern:

```csharp
[HttpGet]
public async Task<IEnumerable<Dto>> Get()
{
    // Get current user info
    var (username, userId) = await GetCurrentUserInfoAsync();
    
    if (!userId.HasValue)
    {
        return Enumerable.Empty<Dto>();
    }
    
    // Get allowed customer IDs based on role coverage
    var allowedCustomerIds = await _userRepo.GetAllowedCustomerIdsAsync(userId.Value);
    
    // Fetch data with coverage filter
    var data = await _repo.GetAllAsync(allowedCustomerIds);
    
    return data.Select(d => ToDto(d));
}
```

## Affected Pages

Coverage filtering is applied to ALL pages that display customer-related data:

- ✅ Customers page
- ✅ Projects page
- ✅ Cases page
- ✅ Orders page
- ✅ PreSales Proposals page
- ✅ PreSales Activities page
- ✅ Requirement Definitions page

## Managing Coverage

Coverage is managed through the Admin page's Coverage tab:

1. Navigate to Admin page
2. Select the "Coverage" tab
3. Click "Manage coverage" for a specific role
4. Check/uncheck customers to assign/remove coverage
5. If all customers are unchecked, the role has unrestricted access to ALL customers

## Testing Coverage

To test the coverage filtering:

1. Create a test role with specific coverage (e.g., only "ログイット")
2. Assign this role to a test user
3. Log in as that user
4. Verify that only "ログイット" appears in:
   - Customer list
   - Project list (only projects for ログイット)
   - Case list (only cases for ログイット)
   - All other lists

## Notes

- Coverage filtering is applied at the repository level for consistency
- The filtering respects other filters (status, date ranges, etc.)
- Users with no roles see NO data
- At least one unrestricted role grants full access to ALL customers
