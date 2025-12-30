using AspireApp1.DbApi.Models;
using AspireApp1.DbApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories;

public class ProjectActivityRepository : IProjectActivityRepository
{
    private readonly ProjectDbContext _db;

    public ProjectActivityRepository(ProjectDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ProjectActivity>> GetAllAsync()
    {
        return await _db.ProjectActivities
            .Include(a => a.Project)
            .Include(a => a.Customer)
            .AsNoTracking()
            .OrderByDescending(a => a.ActivityDate)
            .ToListAsync();
    }

    public async Task<ProjectActivity?> GetAsync(int id)
    {
        return await _db.ProjectActivities
            .Include(a => a.Project)
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<ProjectActivity>> GetByProjectIdAsync(int projectId)
    {
        return await _db.ProjectActivities
            .Include(a => a.Customer)
            .Where(a => a.ProjectId == projectId)
            .AsNoTracking()
            .OrderByDescending(a => a.ActivityDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectActivity>> SearchAsync(
        DateTime? startDate, 
        DateTime? endDate, 
        int? customerId, 
        string? activityType)
    {
        var query = _db.ProjectActivities
            .Include(a => a.Project)
            .Include(a => a.Customer)
            .AsNoTracking()
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(a => a.ActivityDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.ActivityDate <= endDate.Value);
        }

        if (customerId.HasValue)
        {
            query = query.Where(a => a.CustomerId == customerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(activityType))
        {
            query = query.Where(a => a.ActivityType == activityType);
        }

        return await query.OrderByDescending(a => a.ActivityDate).ToListAsync();
    }

    public async Task<ProjectActivity> AddAsync(ProjectActivity activity)
    {
        _db.ProjectActivities.Add(activity);
        await _db.SaveChangesAsync();
        return activity;
    }

    public async Task UpdateAsync(ProjectActivity activity)
    {
        activity.UpdatedAt = DateTime.UtcNow;
        _db.ProjectActivities.Update(activity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var activity = await _db.ProjectActivities.FindAsync(id);
        if (activity != null)
        {
            _db.ProjectActivities.Remove(activity);
            await _db.SaveChangesAsync();
        }
    }
}
