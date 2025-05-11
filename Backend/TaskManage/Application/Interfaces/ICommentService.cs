using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(Comment comment);
        Task<List<Comment>> GetCommentsByTaskIdAsync(int taskId);
        Task<List<Comment>> GetCommentsByUserIdAsync(int userId);
        Task DeleteCommentAsync(int commentId);
    }
}
