using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories
{
    public class PreSalesWorkHourRepository : IPreSalesWorkHourRepository
    {
        private readonly ProjectDbContext _db;
        public PreSalesWorkHourRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<PreSalesWorkHour>> GetAllAsync() =>
            await _db.PreSalesWorkHours
                .Include(w => w.PreSalesProposal)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PreSalesWorkHour>> GetAllAsync(int[]? allowedCustomerIds)
        {
            // If allowedCustomerIds is null, return all work hours (unrestricted access)
            if (allowedCustomerIds == null)
            {
                return await GetAllAsync();
            }

            // If allowedCustomerIds is empty, return no work hours
            if (allowedCustomerIds.Length == 0)
            {
                return Enumerable.Empty<PreSalesWorkHour>();
            }

            // Return only work hours for proposals linked to allowed customers
            return await _db.PreSalesWorkHours
                .Include(w => w.PreSalesProposal)
                .Where(w => allowedCustomerIds.Contains(w.PreSalesProposal.CustomerId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<PreSalesWorkHour>> GetByProposalIdAsync(int proposalId) =>
            await _db.PreSalesWorkHours
                .Include(w => w.PreSalesProposal)
                .Where(w => w.PreSalesProposalId == proposalId)
                .OrderBy(w => w.Title)
                .AsNoTracking()
                .ToListAsync();

        public async Task<PreSalesWorkHour?> GetAsync(int id) =>
            await _db.PreSalesWorkHours
                .Include(w => w.PreSalesProposal)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);

        public async Task<PreSalesWorkHour> AddAsync(PreSalesWorkHour entity)
        {
            _db.PreSalesWorkHours.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(PreSalesWorkHour entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _db.PreSalesWorkHours.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.PreSalesWorkHours.FindAsync(id);
            if (entity != null)
            {
                _db.PreSalesWorkHours.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}

