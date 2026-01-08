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
                .Include(c => c.System)
                .Include(c => c.SystemComponent)
                .Include(c => c.CustomerSite)
                .Include(c => c.CustomerOrder)
                .Include(c => c.AssignedToUser)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Case>> GetByCustomerIdAsync(int customerId) =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.System)
                .Include(c => c.SystemComponent)
                .Include(c => c.CustomerSite)
                .Include(c => c.CustomerOrder)
                .Include(c => c.AssignedToUser)
                .Where(c => c.CustomerId == customerId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Case>> GetByAssignedUserIdAsync(int userId) =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.System)
                .Include(c => c.SystemComponent)
                .Include(c => c.CustomerSite)
                .Include(c => c.CustomerOrder)
                .Include(c => c.AssignedToUser)
                .Where(c => c.AssignedToUserId == userId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Case>> GetByStatusAsync(CaseStatus status) =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.System)
                .Include(c => c.SystemComponent)
                .Include(c => c.CustomerSite)
                .Include(c => c.CustomerOrder)
                .Include(c => c.AssignedToUser)
                .Where(c => c.Status == status)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Case>> GetOverdueCasesAsync() =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.System)
                .Include(c => c.SystemComponent)
                .Include(c => c.CustomerSite)
                .Include(c => c.CustomerOrder)
                .Include(c => c.AssignedToUser)
                .Where(c => c.DueDate.HasValue && c.DueDate.Value < DateTime.UtcNow && 
                           (c.Status == CaseStatus.Open || c.Status == CaseStatus.InProgress))
                .AsNoTracking()
                .ToListAsync();

        public async Task<Case?> GetAsync(int id) =>
            await _db.Cases
                .Include(c => c.Customer)
                .Include(c => c.System)
                .Include(c => c.SystemComponent)
                .Include(c => c.CustomerSite)
                .Include(c => c.CustomerOrder)
                .Include(c => c.AssignedToUser)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Case> AddAsync(Case caseEntity)
        {
            _db.Cases.Add(caseEntity);
            await _db.SaveChangesAsync();
            return caseEntity;
        }

        public async Task<(int? previousAssignedToUserId, CaseStatus previousStatus)> UpdateAsync(Case caseEntity)
        {
            var existing = await _db.Cases.AsNoTracking().FirstOrDefaultAsync(c => c.Id == caseEntity.Id);
            var previousAssignedToUserId = existing?.AssignedToUserId;
            var previousStatus = existing?.Status ?? CaseStatus.Open;
            
            _db.Cases.Update(caseEntity);
            await _db.SaveChangesAsync();
            
            return (previousAssignedToUserId, previousStatus);
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

        public async Task<int> GetOpenRelatedCasesCountAsync(int caseId)
        {
            return await _db.CaseRelationships
                .Where(cr => (cr.SourceCaseId == caseId || cr.RelatedCaseId == caseId))
                .Select(cr => cr.SourceCaseId == caseId ? cr.RelatedCaseId : cr.SourceCaseId)
                .Distinct()
                .Join(_db.Cases, 
                      relatedId => relatedId,
                      c => c.Id,
                      (relatedId, c) => c)
                .Where(c => c.Status != CaseStatus.Closed && c.Status != CaseStatus.Resolved)
                .CountAsync();
        }

        public async Task<int[]> BulkUpdateAssignmentAsync(int[] caseIds, int? assignedToUserId)
        {
            var cases = await _db.Cases.Where(c => caseIds.Contains(c.Id)).ToListAsync();
            foreach (var c in cases)
            {
                c.AssignedToUserId = assignedToUserId;
                c.UpdatedAt = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync();
            return cases.Select(c => c.Id).ToArray();
        }

        public async Task<int[]> BulkUpdateStatusAsync(int[] caseIds, CaseStatus status)
        {
            var cases = await _db.Cases.Where(c => caseIds.Contains(c.Id)).ToListAsync();
            foreach (var c in cases)
            {
                c.Status = status;
                c.UpdatedAt = DateTime.UtcNow;
                
                if (status == CaseStatus.Resolved || status == CaseStatus.Closed)
                {
                    if (!c.ResolvedAt.HasValue)
                        c.ResolvedAt = DateTime.UtcNow;
                    
                    if (status == CaseStatus.Closed && !c.ClosedAt.HasValue)
                        c.ClosedAt = DateTime.UtcNow;
                }
            }
            await _db.SaveChangesAsync();
            return cases.Select(c => c.Id).ToArray();
        }
    }
}
