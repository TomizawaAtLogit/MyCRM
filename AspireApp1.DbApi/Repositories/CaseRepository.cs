using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories
{
    public class CaseRepository : ICaseRepository
    {
        private readonly ProjectDbContext _db;
        public CaseRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<Case>> GetAllAsync() =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.AssignedToUser)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Case>> GetByCustomerIdAsync(int customerId) =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.AssignedToUser)
                .Where(c => c.CustomerId == customerId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Case>> GetByAssignedUserIdAsync(int userId) =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.AssignedToUser)
                .Where(c => c.AssignedToUserId == userId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Case>> GetByStatusAsync(CaseStatus status) =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.AssignedToUser)
                .Where(c => c.Status == status)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Case>> GetOverdueCasesAsync() =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.AssignedToUser)
                .Where(c => c.DueDate.HasValue && c.DueDate.Value < DateTime.UtcNow && 
                           (c.Status == CaseStatus.Open || c.Status == CaseStatus.InProgress))
                .AsNoTracking()
                .ToListAsync();

        public async Task<Case?> GetAsync(int id) =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.AssignedToUser)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Case> AddAsync(Case caseEntity)
        {
            _db.Cases.Add(caseEntity);
            await _db.SaveChangesAsync();
            return caseEntity;
        }

        public async Task UpdateAsync(Case caseEntity)
        {
            _db.Cases.Update(caseEntity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _db.Cases.FindAsync(id);
            if (c != null)
            {
                _db.Cases.Remove(c);
                await _db.SaveChangesAsync();
            }
        }
    }
}
