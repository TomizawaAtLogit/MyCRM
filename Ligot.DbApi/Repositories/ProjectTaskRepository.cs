using Ligot.DbApi.Data;
using Ligot.DbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligot.DbApi.Repositories
{
    public class ProjectTaskRepository : IProjectTaskRepository
    {
        private readonly ProjectDbContext _db;
        public ProjectTaskRepository(ProjectDbContext db) => _db = db;

        public async Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(int projectId) =>
            await _db.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.DisplayOrder)
                .ThenBy(t => t.StartAtUtc)
                .AsNoTracking()
                .ToListAsync();

        public async Task<ProjectTask?> GetAsync(int id) =>
            await _db.ProjectTasks.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<ProjectTask> AddAsync(ProjectTask task)
        {
            _db.ProjectTasks.Add(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(ProjectTask task)
        {
            task.UpdatedAt = DateTime.UtcNow;
            _db.ProjectTasks.Update(task);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _db.ProjectTasks.FindAsync(id);
            if (task != null)
            {
                _db.ProjectTasks.Remove(task);
                await _db.SaveChangesAsync();
            }
        }
    }
}

