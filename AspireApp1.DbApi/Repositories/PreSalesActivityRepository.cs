using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories
{
    public class PreSalesActivityRepository : IPreSalesActivityRepository
    {
        private readonly ProjectDbContext _db;
        public PreSalesActivityRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<PreSalesActivity>> GetAllAsync() =>
            await _db.PreSalesActivities
                .Include(a => a.PreSalesProposal)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PreSalesActivity>> GetByProposalIdAsync(int proposalId) =>
            await _db.PreSalesActivities
                .Include(a => a.PreSalesProposal)
                .Where(a => a.PreSalesProposalId == proposalId)
                .OrderByDescending(a => a.ActivityDate)
                .AsNoTracking()
                .ToListAsync();

        public async Task<PreSalesActivity?> GetAsync(int id) =>
            await _db.PreSalesActivities
                .Include(a => a.PreSalesProposal)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<PreSalesActivity> AddAsync(PreSalesActivity entity)
        {
            _db.PreSalesActivities.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(PreSalesActivity entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _db.PreSalesActivities.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.PreSalesActivities.FindAsync(id);
            if (entity != null)
            {
                _db.PreSalesActivities.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
