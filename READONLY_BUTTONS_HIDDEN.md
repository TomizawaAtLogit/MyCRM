# Read-Only Permission Button Hiding Implementation

## Summary

Implemented a system-wide solution to hide create, edit, and delete buttons when users have "Read Only" permissions on pages. This prevents users from attempting to modify data they don't have permission to change.

## What Was Changed

### 1. **Backend - Authorization Service** 
**File:** `AspireApp1.Web/Services/AuthorizationService.cs`

Added a new method `IsPageReadOnlyAsync()` that:
- Checks if the current user has ReadOnly (not FullControl) permission for a specific page
- Returns `true` if the user has ReadOnly permission
- Returns `false` if the user has FullControl or no permission
- Parses permissions in both new format (`"Page:ReadOnly"`) and legacy format (`"Page"`)

```csharp
public async Task<bool> IsPageReadOnlyAsync(string pageName)
{
    // Checks user roles for ReadOnly permission level on specified page
}
```

### 2. **Pages Updated with Button Hiding**

#### **Orders.razor** (`AspireApp1.Web/Components/Pages/Orders.razor`)
- Added `AuthorizationService` injection
- Added `isReadOnly` field loaded during initialization
- Hidden buttons when readonly:
  - ✅ Create Order button (top toolbar)
  - ✅ Edit button (pencil icon in table)
  - ✅ Delete button (trash icon in table)
  - ℹ️ View Details button (eye icon) - always visible

#### **Projects.razor** (`AspireApp1.Web/Components/Pages/Projects.razor`)
- Added `AuthorizationService` injection
- Added `isReadOnly` field loaded during initialization
- Hidden buttons when readonly:
  - ✅ Create Project button (top toolbar)
  - ✅ Edit button (pencil icon in table)
  - ✅ Delete button (trash icon in table)
  - ℹ️ View Details button (eye icon) - always visible

#### **Cases.razor** (`AspireApp1.Web/Components/Pages/Cases.razor`)
- Added `AuthorizationService` injection
- Added `isReadOnly` field loaded during initialization
- Hidden buttons when readonly:
  - ✅ Create Case button (top toolbar)
  - ✅ Edit button (pencil icon in table)
  - ✅ Delete button (trash icon in table)
  - ℹ️ View Details button (eye icon) - always visible

#### **Customers.razor** (`AspireApp1.Web/Components/Pages/Customers.razor`)
- Added `AuthorizationService` injection
- Added `isReadOnly` field loaded during initialization
- Hidden buttons when readonly:
  - ✅ Create Customer form and toggle button
  - ✅ Edit button (pencil icon)
  - ✅ Delete button (trash icon)
  - Passes `IsReadOnly` parameter to OrderTab sub-component

#### **OrderTab.razor** (`AspireApp1.Web/Components/Pages/OrderTab.razor`)
- Added `IsReadOnly` parameter (receives from parent Customers component)
- Hidden buttons when readonly:
  - ✅ Add Order form and toggle button
  - ✅ Edit button (pencil icon in table)
  - ✅ Delete button (trash icon in table)

### 3. **Implementation Pattern**

Each page follows this pattern:

```csharp
// In @code block
private bool isReadOnly = false;

protected override async Task OnInitializedAsync()
{
    // ... other initialization ...
    isReadOnly = await AuthService.IsPageReadOnlyAsync("PageName");
    // ... rest of initialization ...
}
```

In the UI, buttons are conditionally rendered:

```html
@if (!isReadOnly)
{
    <button class="btn btn-primary" @onclick="ShowCreateModal">
        <i class="bi bi-plus-circle me-2"></i>Create Order
    </button>
}
```

## Permission System Overview

The system distinguishes between permission levels:

- **None**: User cannot access the page (blocked by `PageAccessGuard`)
- **ReadOnly**: User can view but NOT create/edit/delete (buttons hidden)
- **FullControl**: User has full access (all buttons visible)

Permission format in database: `"Page:PermissionLevel"`
Examples: `"Orders:ReadOnly"`, `"Projects:FullControl"`, `"Cases:None"`

## User Experience

### Before This Change
1. User with ReadOnly permission navigates to Orders page
2. User sees Create, Edit, Delete buttons
3. User clicks Edit button
4. Backend rejects the request (401 Unauthorized)
5. User sees error message

### After This Change
1. User with ReadOnly permission navigates to Orders page
2. User sees only View (eye icon) button
3. User cannot attempt to edit/delete (buttons are hidden)
4. User has clear visual indication of what they can do

## Testing the Changes

1. **Set a user to ReadOnly for Orders:**
   - Go to Admin > Permissions
   - Select a user role
   - Set Orders permission to "Read only"
   - Save changes

2. **Verify buttons are hidden:**
   - Navigate to Orders page
   - Create button should NOT appear in top toolbar
   - Edit (pencil) and Delete (trash) buttons should NOT appear in table rows
   - View (eye) button should still be visible

3. **Verify buttons appear for FullControl:**
   - Change permission to "Full Control"
   - Refresh page
   - All buttons should now be visible

## Build Status

✅ **Clean Build**: No errors or warnings
✅ **All Changes Applied Successfully**

## Files Modified

1. `AspireApp1.Web/Services/AuthorizationService.cs` - Added `IsPageReadOnlyAsync()` method
2. `AspireApp1.Web/Components/Pages/Orders.razor` - Hide buttons when readonly
3. `AspireApp1.Web/Components/Pages/Projects.razor` - Hide buttons when readonly
4. `AspireApp1.Web/Components/Pages/Cases.razor` - Hide buttons when readonly
5. `AspireApp1.Web/Components/Pages/Customers.razor` - Hide buttons when readonly
6. `AspireApp1.Web/Components/Pages/OrderTab.razor` - Added IsReadOnly parameter

## Future Enhancements

Consider applying similar changes to other pages/components:
- PreSales pages
- Audit pages
- Any other pages with create/edit/delete operations

The pattern is consistent and can be easily replicated.
