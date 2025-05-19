using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Entities.TaskStatus;

namespace Infrastructure.Repository {
    public class TaskRepository(TaskManageDbContext dbContext) : ITaskRepository {
        public Task<TaskNode?> GetNodeById(int id) {
            return dbContext.TaskNodes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(TaskNode task) {
            dbContext.TaskNodes.Add(task);
            await dbContext.SaveChangesAsync();
            return task.Id;
        }

        public async Task UpdateAsync(TaskNode task) {
            dbContext.TaskNodes.Update(task);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var res = (await dbContext.TaskNodes.FirstOrDefaultAsync(x => x.Id == id));
            ArgumentNullException.ThrowIfNull(res);

            dbContext.TaskNodes.Remove(res);
            await dbContext.SaveChangesAsync();
        }

        public Task<List<TaskNode>> GetTasksByStatus(TaskStatus status) {
            return dbContext.TaskNodes.Where(x => x.TaskStatus == status).ToListAsync();
        }

       
        public Task<List<TaskNode>> GetAllTasksByProjectId(int projectId) {
            return dbContext.TaskNodes.Include(x=>x.DependentNodes).Where(x => x.Project.Id == projectId).ToListAsync();
        }
    }
}
