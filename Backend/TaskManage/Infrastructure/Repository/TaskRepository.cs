using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repository;
using Entities;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Entities.TaskStatus;

namespace Infrastructure.Repository {
    public class TaskRepository(TaskManageDbContext dbContext) : ITaskRepository {
        public Task<TaskNode?> GetNodeById(int id) {
            return dbContext.TaskNodes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetAssignedUser(int id) {
            return (await dbContext.TaskNodes.FirstOrDefaultAsync(x => x.Id == id))?.AssignedUser;
        }

        public async Task AddAsync(TaskNode task) {
            dbContext.TaskNodes.Add(task);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskNode task) {
            dbContext.TaskNodes.Update(task);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var res = (await dbContext.TaskNodes.FirstAsync(x => x.Id == id));
            dbContext.TaskNodes.Remove(res);
            await dbContext.SaveChangesAsync();
        }

        public Task<List<TaskNode>> GetTasksByStatus(TaskStatus status) {
            return dbContext.TaskNodes.Where(x => x.TaskStatus == status).ToListAsync();
        }
    }
}
