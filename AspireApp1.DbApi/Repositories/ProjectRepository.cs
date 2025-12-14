using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp1.DbApi.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectDbContext _db;
        public ProjectRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<Project>> GetAllAsync() =>
            await _db.Projects.AsNoTracking().ToListAsync();

        public async Task<Project?> GetAsync(int id) =>
            await _db.Projects.FindAsync(id);

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
