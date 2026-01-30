using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories;

public class CaseRelationshipRepository : ICaseRelationshipRepository
{
    private readonly ProjectDbContext _db;
    public CaseRelationshipRepository(ProjectDbContext db) => _db = db;

    public async Task<IEnumerable<CaseRelationship>> GetByCaseIdAsync(int caseId) =>
        await _db.CaseRelationships
            .Include(cr => cr.SourceCase)
            .Include(cr => cr.RelatedCase)
            .Where(cr => cr.SourceCaseId == caseId || cr.RelatedCaseId == caseId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<CaseRelationship?> GetAsync(int id) =>
        await _db.CaseRelationships
            .Include(cr => cr.SourceCase)
            .Include(cr => cr.RelatedCase)
            .FirstOrDefaultAsync(cr => cr.Id == id);

    public async Task<CaseRelationship> AddAsync(CaseRelationship relationship)
    {
        _db.CaseRelationships.Add(relationship);
        await _db.SaveChangesAsync();
        return relationship;
    }

    public async Task DeleteAsync(int id)
    {
        var relationship = await _db.CaseRelationships.FindAsync(id);
        if (relationship != null)
        {
            _db.CaseRelationships.Remove(relationship);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteBidirectionalAsync(int sourceCaseId, int relatedCaseId, CaseRelationshipType relationshipType)
    {
        var relationships = await _db.CaseRelationships
            .Where(cr => 
                (cr.SourceCaseId == sourceCaseId && cr.RelatedCaseId == relatedCaseId && cr.RelationshipType == relationshipType) ||
                (cr.SourceCaseId == relatedCaseId && cr.RelatedCaseId == sourceCaseId && cr.RelationshipType == relationshipType))
            .ToListAsync();
        
        _db.CaseRelationships.RemoveRange(relationships);
        await _db.SaveChangesAsync();
    }
}

