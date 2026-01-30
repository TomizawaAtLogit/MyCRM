using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories
{
    public class RequirementDefinitionRepository : IRequirementDefinitionRepository
    {
        private readonly ProjectDbContext _db;
        public RequirementDefinitionRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<RequirementDefinition>> GetAllAsync() =>
            await _db.RequirementDefinitions
                .Include(r => r.Customer)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<RequirementDefinition>> GetAllAsync(int[]? allowedCustomerIds)
        {
            // If allowedCustomerIds is null, return all requirement definitions (unrestricted access)
            if (allowedCustomerIds == null)
            {
                return await GetAllAsync();
            }

            // If allowedCustomerIds is empty, return no requirement definitions
            if (allowedCustomerIds.Length == 0)
            {
                return Enumerable.Empty<RequirementDefinition>();
            }

            // Return only requirement definitions for allowed customers
            return await _db.RequirementDefinitions
                .Include(r => r.Customer)
                .Where(r => allowedCustomerIds.Contains(r.CustomerId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<RequirementDefinition>> GetByCustomerIdAsync(int customerId) =>
            await _db.RequirementDefinitions
                .Include(r => r.Customer)
                .Where(r => r.CustomerId == customerId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<RequirementDefinition?> GetAsync(int id) =>
            await _db.RequirementDefinitions
                .Include(r => r.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<RequirementDefinition> AddAsync(RequirementDefinition entity)
        {
            _db.RequirementDefinitions.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(RequirementDefinition entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _db.RequirementDefinitions.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.RequirementDefinitions.FindAsync(id);
            if (entity != null)
            {
                _db.RequirementDefinitions.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}

