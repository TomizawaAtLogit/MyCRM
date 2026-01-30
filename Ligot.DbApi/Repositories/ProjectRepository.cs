using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectDbContext _db;
        public ProjectRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<Project>> GetAllAsync() =>
            await _db.Projects.Include(p => p.Customer).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Project>> GetAllAsync(int[]? allowedCustomerIds)
        {
            // If allowedCustomerIds is null, return all projects (unrestricted access)
            if (allowedCustomerIds == null)
            {
                return await GetAllAsync();
            }

            // If allowedCustomerIds is empty, return no projects
            if (allowedCustomerIds.Length == 0)
            {
                return Enumerable.Empty<Project>();
            }

            // Return only projects for allowed customers
            return await _db.Projects
                .Include(p => p.Customer)
                .Where(p => allowedCustomerIds.Contains(p.CustomerId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetByCustomerIdAsync(int customerId) =>
            await _db.Projects.Include(p => p.Customer).Where(p => p.CustomerId == customerId).AsNoTracking().ToListAsync();

        public async Task<Project?> GetAsync(int id) =>
            await _db.Projects.Include(p => p.Customer).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Project> AddAsync(Project project)
        {
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return project;
        }

        public async Task UpdateAsync(Project project)
        {
            _db.Projects.Update(project);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _db.Projects.FindAsync(id);
            if (p != null)
            {
                _db.Projects.Remove(p);
                await _db.SaveChangesAsync();
            }
        }
    }
}

