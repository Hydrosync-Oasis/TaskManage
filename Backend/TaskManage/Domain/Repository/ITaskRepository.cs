using Domain.Entities;
using TaskStatus = Domain.Entities.TaskStatus;

namespace Domain.Repository {
    public interface ITaskRepository {
        public Task<TaskNode?> GetNodeById(int id);
        Task<int> AddAsync(TaskNode task);
        Task UpdateAsync(TaskNode task);
        Task DeleteAsync(int id);
        Task<List<TaskNode>> GetAllTasksByProjectId(int projectId);

        Task<List<TaskNode>> GetTasksByStatus(TaskStatus  status);

        Task<IEnumerable<TaskNode>> GetTasksByProjectIdAsync(int projectId);
        Task<TaskNode> AddTaskToProjectAsync(TaskNode task);
        Task<bool> RemoveTaskFromProjectAsync(int projectId, int taskId);

    }
}
