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

        public async Task<IEnumerable<PreSalesActivity>> GetAllAsync(int[]? allowedCustomerIds)
        {
            // If allowedCustomerIds is null, return all activities (unrestricted access)
            if (allowedCustomerIds == null)
            {
                return await GetAllAsync();
            }

            // If allowedCustomerIds is empty, return no activities
            if (allowedCustomerIds.Length == 0)
            {
                return Enumerable.Empty<PreSalesActivity>();
            }

            // Return only activities for proposals linked to allowed customers
            return await _db.PreSalesActivities
                .Include(a => a.PreSalesProposal)
                .Where(a => allowedCustomerIds.Contains(a.PreSalesProposal.CustomerId))
                .AsNoTracking()
                .ToListAsync();
        }

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
