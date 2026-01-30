using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories;

public class SlaConfigurationRepository : ISlaConfigurationRepository
{
    private readonly ProjectDbContext _db;
    public SlaConfigurationRepository(ProjectDbContext db) => _db = db;

    public async Task<IEnumerable<SlaThreshold>> GetAllAsync() =>
        await _db.SlaThresholds.AsNoTracking().ToListAsync();

    public async Task<SlaThreshold?> GetByPriorityAsync(CasePriority priority) =>
        await _db.SlaThresholds.FirstOrDefaultAsync(s => s.Priority == priority && s.IsActive);

    public async Task<SlaThreshold?> GetAsync(int id) =>
        await _db.SlaThresholds.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<SlaThreshold> AddAsync(SlaThreshold config)
    {
        _db.SlaThresholds.Add(config);
        await _db.SaveChangesAsync();
        return config;
    }

    public async Task UpdateAsync(SlaThreshold config)
    {
        _db.SlaThresholds.Update(config);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var config = await _db.SlaThresholds.FindAsync(id);
        if (config != null)
        {
            _db.SlaThresholds.Remove(config);
            await _db.SaveChangesAsync();
        }
    }
}

