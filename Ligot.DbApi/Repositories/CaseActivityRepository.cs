using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories
{
    public class CaseActivityRepository : ICaseActivityRepository
    {
        private readonly ProjectDbContext _db;
        public CaseActivityRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<CaseActivity>> GetByCaseIdAsync(int caseId) =>
            await _db.CaseActivities
                .Where(a => a.CaseId == caseId)
                .OrderByDescending(a => a.ActivityDate)
                .AsNoTracking()
                .ToListAsync();

        public async Task<CaseActivity?> GetAsync(int id) =>
            await _db.CaseActivities.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<CaseActivity> AddAsync(CaseActivity activity)
        {
            _db.CaseActivities.Add(activity);
            await _db.SaveChangesAsync();
            return activity;
        }

        public async Task UpdateAsync(CaseActivity activity)
        {
            _db.CaseActivities.Update(activity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var activity = await _db.CaseActivities.FindAsync(id);
            if (activity != null)
            {
                _db.CaseActivities.Remove(activity);
                await _db.SaveChangesAsync();
            }
        }
    }
}

