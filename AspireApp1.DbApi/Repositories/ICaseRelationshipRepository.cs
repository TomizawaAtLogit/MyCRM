using AspireApp1.DbApi.Models;

namespace AspireApp1.DbApi.Repositories;

public interface ICaseRelationshipRepository
{
    Task<IEnumerable<CaseRelationship>> GetByCaseIdAsync(int caseId);
    Task<CaseRelationship?> GetAsync(int id);
    Task<CaseRelationship> AddAsync(CaseRelationship relationship);
    Task DeleteAsync(int id);
    Task DeleteBidirectionalAsync(int sourceCaseId, int relatedCaseId, CaseRelationshipType relationshipType);
}
