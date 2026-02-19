using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories
{
    public class PreSalesProposalRepository : IPreSalesProposalRepository
    {
        private readonly ProjectDbContext _db;
        public PreSalesProposalRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<PreSalesProposal>> GetAllAsync() =>
            await _db.PreSalesProposals
                .Include(p => p.Customer)
                .Include(p => p.RequirementDefinition)
                .Include(p => p.AssignedToUser)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PreSalesProposal>> GetAllAsync(int[]? allowedCustomerIds)
        {
            // If allowedCustomerIds is null, return all proposals (unrestricted access)
            if (allowedCustomerIds == null)
            {
                return await GetAllAsync();
            }

            // If allowedCustomerIds is empty, return no proposals
            if (allowedCustomerIds.Length == 0)
            {
                return Enumerable.Empty<PreSalesProposal>();
            }

            // Return only proposals for allowed customers
            return await _db.PreSalesProposals
                .Include(p => p.Customer)
                .Include(p => p.RequirementDefinition)
                .Include(p => p.AssignedToUser)
                .Where(p => allowedCustomerIds.Contains(p.CustomerId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<PreSalesProposal>> GetByCustomerIdAsync(int customerId) =>
            await _db.PreSalesProposals
                .Include(p => p.Customer)
                .Include(p => p.RequirementDefinition)
                .Include(p => p.AssignedToUser)
                .Where(p => p.CustomerId == customerId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PreSalesProposal>> GetByAssignedUserIdAsync(int userId) =>
            await _db.PreSalesProposals
                .Include(p => p.Customer)
                .Include(p => p.RequirementDefinition)
                .Include(p => p.AssignedToUser)
                .Where(p => p.AssignedToUserId == userId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PreSalesProposal>> GetByStatusAsync(PreSalesStatus status) =>
            await _db.PreSalesProposals
                .Include(p => p.Customer)
                .Include(p => p.RequirementDefinition)
                .Include(p => p.AssignedToUser)
                .Where(p => p.Status == status)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PreSalesProposal>> GetByStageAsync(PreSalesStage stage) =>
            await _db.PreSalesProposals
                .Include(p => p.Customer)
                .Include(p => p.RequirementDefinition)
                .Include(p => p.AssignedToUser)
                .Where(p => p.Stage == stage)
                .AsNoTracking()
                .ToListAsync();

        public async Task<PreSalesProposal?> GetAsync(int id) =>
            await _db.PreSalesProposals
                .Include(p => p.Customer)
                .Include(p => p.RequirementDefinition)
                .Include(p => p.AssignedToUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<PreSalesProposal?> GetAsyncForUpdateAsync(int id) =>
            await _db.PreSalesProposals
                .Include(p => p.Customer)
                .Include(p => p.RequirementDefinition)
                .Include(p => p.AssignedToUser)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<PreSalesProposal> AddAsync(PreSalesProposal entity)
        {
            _db.PreSalesProposals.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(PreSalesProposal entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _db.PreSalesProposals.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.PreSalesProposals.FindAsync(id);
            if (entity != null)
            {
                _db.PreSalesProposals.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
