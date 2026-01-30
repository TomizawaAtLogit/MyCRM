# Dashboard Metrics Data Collection - Explanation

## Current Implementation

### How Dashboard Metrics Are Gathered

The dashboard metrics are **calculated on-demand** and **not automatically scheduled at night**. Here's how it works:

#### 1. **On-Demand Calculation (Current)**
- When you load the Dashboard page (`/dashboard`), the frontend calls `DashboardApi.GetCurrentMetricsAsync()`
- This hits the API endpoint: `GET /api/dashboard/current`
- The `DashboardController.GetCurrentMetrics()` method:
  - Gets the current user's ID
  - Retrieves the user's role
  - Calls `IDashboardService.GetCurrentMetricsAsync(roleId)` to calculate metrics
  - The `DashboardService.CalculateMetricsAsync()` method:
    - Queries the database for all Pre-sales, Cases, and Projects data
    - Counts and calculates various metrics based on RoleCoverage
    - Returns calculations **without storing them** (just returns current state)

#### 2. **Snapshot Storage (Optional/Manual)**
- There IS an endpoint to manually generate and store snapshots: `POST /api/dashboard/snapshot`
- This endpoint:
  - Gets all roles
  - For each role, calculates current metrics
  - **Stores the snapshot** in the `DashboardMetrics` table with timestamp
  - Can be used to track historical trends

#### 3. **Historical Data Retrieval**
- Endpoint: `GET /api/dashboard/history` or `GET /api/dashboard/history/customer/{customerId}`
- Returns stored snapshots from the last N days (default 30 days)
- Only works if snapshots were previously generated and stored

---

## Key Files Involved

| File | Purpose |
|------|---------|
| [Ligot.DbApi/Controllers/DashboardController.cs](Ligot.DbApi/Controllers/DashboardController.cs) | API endpoints for dashboard operations |
| [Ligot.DbApi/Services/DashboardService.cs](Ligot.DbApi/Services/DashboardService.cs) | Business logic for calculating metrics |
| [Ligot.Web/Components/Pages/Dashboard.razor](Ligot.Web/Components/Pages/Dashboard.razor) | Frontend dashboard page |
| [Ligot.Web/DashboardApiClient.cs](Ligot.Web/DashboardApiClient.cs) | API client for frontend |

---

## What Happens at Night?

**Currently: Nothing automatic for dashboard metrics.**

The only background job that runs is `AuditCleanupService` (runs daily):
- Checks daily for expired audit logs
- Deletes logs older than retention period
- Does NOT affect dashboard metrics

---

## How to Enable Manual/Scheduled Snapshots

### Option 1: Manual Snapshot Generation
You can manually trigger snapshot generation via API:

```bash
POST /api/dashboard/snapshot
```

This will:
1. Calculate current metrics for all roles
2. Store them in `DashboardMetrics` table
3. Allow you to view historical trends later

You could create a PowerShell script to call this periodically.

### Option 2: Add Scheduled Background Job
To automatically generate snapshots at specific times (e.g., nightly), you would:

1. Create a new `DashboardSnapshotService : BackgroundService` similar to `AuditCleanupService`
2. Configure it to run at specific times (e.g., 2 AM)
3. Register it in `Program.cs`: `builder.Services.AddHostedService<DashboardSnapshotService>();`

Example implementation would be:
```csharp
public class DashboardSnapshotService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DashboardSnapshotService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);
    private readonly int _snapshotHour = 2; // 2 AM UTC

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            if (now.Hour == _snapshotHour && now.Minute < 5)
            {
                try
                {
                    await GenerateSnapshotsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating dashboard snapshots");
                }
            }
            
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task GenerateSnapshotsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();
        var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepository>();

        var roles = await roleRepo.GetAllAsync();
        foreach (var role in roles)
        {
            await dashboardService.GenerateSnapshotAsync(role.Id);
        }
        
        _logger.LogInformation("Dashboard snapshots generated at {Timestamp} UTC", now);
    }
}
```

---

## Recommendation

Based on your concern about night-time data collection:

1. **Current Behavior**: Metrics are calculated **when you load the page** (on-demand), not at night
2. **If you want historical tracking**: Use the snapshot feature to periodically save metrics
3. **If you want automatic nightly snapshots**: Add the `DashboardSnapshotService` (shown above) to run at your preferred time
4. **For manual control**: Create a PowerShell script that calls `POST /api/dashboard/snapshot` when you want to capture a snapshot

---

## Questions?

- Do you want to enable automatic nightly snapshots?
- Do you want to trigger manual snapshots from the UI?
- Do you need to adjust when/how snapshots are created?

