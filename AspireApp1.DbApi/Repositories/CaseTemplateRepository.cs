using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories;

public class CaseTemplateRepository : ICaseTemplateRepository
{
    private readonly ProjectDbContext _db;
    public CaseTemplateRepository(ProjectDbContext db) => _db = db;

    public async Task<IEnumerable<CaseTemplate>> GetAllActiveAsync() =>
        await _db.CaseTemplates
            .Where(t => t.IsActive)
            .OrderBy(t => t.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<CaseTemplate>> GetAllAsync() =>
        await _db.CaseTemplates.AsNoTracking().ToListAsync();

    public async Task<CaseTemplate?> GetAsync(int id) =>
        await _db.CaseTemplates.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<CaseTemplate> AddAsync(CaseTemplate template)
    {
        _db.CaseTemplates.Add(template);
        await _db.SaveChangesAsync();
        return template;
    }

    public async Task UpdateAsync(CaseTemplate template)
    {
        _db.CaseTemplates.Update(template);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var template = await _db.CaseTemplates.FindAsync(id);
        if (template != null)
        {
            _db.CaseTemplates.Remove(template);
            await _db.SaveChangesAsync();
        }
    }
}
