using Domain.Entities;
using TaskStatus = Domain.Entities.TaskStatus;

namespace Domain.Repository {
    public interface ITaskRepository {
        public Task<TaskNode?> GetNodeById(int id);
        Task AddAsync(TaskNode task);
        Task UpdateAsync(TaskNode task);
        Task DeleteAsync(int id);
        Task<List<TaskNode>> GetAllTasksByProjectId(int projectId);

        Task<List<TaskNode>> GetTasksByStatus(TaskStatus  status);

        // 任务节点的评论功能，评论被聚合在了任务节点中

        public Task<List<Comment>> GetAllCommentsByTaskIdAsync(int id);

        public Task DeleteByCommentIdAsync(int id);
        public Task AddAsync(Comment comment);

    }
}
