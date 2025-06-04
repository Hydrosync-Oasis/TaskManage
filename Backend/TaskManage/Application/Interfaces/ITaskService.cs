using Application.Dtos;

namespace Application.Interfaces
{
    public interface ITaskService
    {
        public Task UpdateTask(TaskDto dto);

        /// <summary>
        /// 创建一个任务
        /// </summary>
        /// <param name="dto">任务描述对象</param>
        /// <param name="uid">创建者id</param>
        /// <returns></returns>
        public Task<int> AddTask(TaskDto dto, int uid);

        public Task RemoveTask(int taskId);

        public Task<TaskDto> GetTaskInfo(int taskId);

        public Task<bool> IsTaskExists(int taskId);

        public Task AddCommentAsync(CommentDto comment);

        public Task<CommentDto?> GetCommentByIdAsync(int id);     // 通过评论ID获取评论

        public Task DeleteCommentAsync(int id);               // 通过评论ID删除评论

        public Task<List<CommentDto>> GetAllCommentsByTaskIdAsync(int taskId);                //通过任务ID获取所有评论
    }
}

