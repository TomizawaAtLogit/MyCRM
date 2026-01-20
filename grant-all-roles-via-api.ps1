param(
    [string]$ApiUrl = "https://localhost:7030"
)

Write-Host "Granting all roles to tomizawa user via API..."
Write-Host "API URL: $ApiUrl"

# Suppress SSL certificate warnings for local development
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Get all users
    Write-Host "`n[1/3] Fetching user list..."
    $usersResponse = Invoke-RestMethod -Uri "$ApiUrl/api/admin/users" -Method Get -ErrorAction Stop
    $tomizawaUser = $usersResponse | Where-Object { $_.windowsUsername -eq "tomizawa" }
    
    if (-not $tomizawaUser) {
        Write-Host "✗ Error: User 'tomizawa' not found in database" -ForegroundColor Red
        Write-Host "Available users:"
        $usersResponse | ForEach-Object { Write-Host "  - $($_.windowsUsername) ($($_.displayName))" }
        exit 1
    }
    
    Write-Host "✓ Found user: $($tomizawaUser.displayName) (ID: $($tomizawaUser.id))"
    
    # Get all roles
    Write-Host "`n[2/3] Fetching role list..."
    $rolesResponse = Invoke-RestMethod -Uri "$ApiUrl/api/admin/roles" -Method Get -ErrorAction Stop
    Write-Host "✓ Found $($rolesResponse.Count) roles:"
    $rolesResponse | ForEach-Object { Write-Host "  - $($_.name) (ID: $($_.id))" }
    
    # Assign each role to the user
    Write-Host "`n[3/3] Assigning roles to user..."
    foreach ($role in $rolesResponse) {
        try {
            Invoke-RestMethod -Uri "$ApiUrl/api/admin/users/$($tomizawaUser.id)/roles/$($role.id)" -Method Post -ErrorAction Stop | Out-Null
            Write-Host "  ✓ Assigned role: $($role.name)"
        }
        catch {
            # If role already assigned (409 Conflict), that's ok
            if ($_.Exception.Response.StatusCode -eq 409) {
                Write-Host "  ✓ Role already assigned: $($role.name)"
            } else {
                Write-Host "  ✗ Failed to assign role $($role.name): $_" -ForegroundColor Yellow
            }
        }
    }
    
    # Verify the results
    Write-Host "`n[4/4] Verifying user roles..."
    $verifyUser = Invoke-RestMethod -Uri "$ApiUrl/api/admin/users/by-username/tomizawa" -Method Get -ErrorAction Stop
    
    Write-Host "`n✓ User Configuration:"
    Write-Host "  Username: $($verifyUser.windowsUsername)"
    Write-Host "  Display Name: $($verifyUser.displayName)"
    Write-Host "  Active: $($verifyUser.isActive)"
    Write-Host "  Assigned Roles:"
    if ($verifyUser.userRoles -and $verifyUser.userRoles.Count -gt 0) {
        $verifyUser.userRoles | ForEach-Object { 
            Write-Host "    • $($_.role.name): $($_.role.pagePermissions)"
        }
    } else {
        Write-Host "    (No roles assigned - check if API returned role data)"
    }
    
    Write-Host "`n✓ All roles have been assigned successfully!" -ForegroundColor Green
    
}
catch {
    if ($_.Exception.Response.StatusCode -eq 401) {
        Write-Host "✗ Authentication required - the application may have authorization enabled" -ForegroundColor Red
    }
    Write-Host "✗ Error: $_" -ForegroundColor Red
    Write-Host "Make sure the application is running at $ApiUrl" -ForegroundColor Yellow
    exit 1
}
