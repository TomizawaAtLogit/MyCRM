using AspireApp1.DbApi.Repositories;

namespace AspireApp1.DbApi.Services;

public class AuditCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditCleanupService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1); // Check daily

    public AuditCleanupService(IServiceProvider serviceProvider, ILogger<AuditCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Audit Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredLogsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during audit log cleanup");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Audit Cleanup Service stopped");
    }

    private async Task CleanupExpiredLogsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var auditRepo = scope.ServiceProvider.GetRequiredService<IAuditRepository>();

        var now = DateTime.UtcNow;
        var deletedCount = await auditRepo.DeleteExpiredAsync(now);

        if (deletedCount > 0)
        {
            // Log to console as per requirements
            Console.WriteLine($"[Audit Cleanup] Deleted {deletedCount} expired audit logs at {now:yyyy-MM-dd HH:mm:ss} UTC");
            _logger.LogInformation("Deleted {Count} expired audit logs", deletedCount);
        }
    }
}
