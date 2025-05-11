using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(Comment comment);         // 添加评论（仍需 Comment 对象）
        Task<Comment> GetCommentByIdAsync(int id);     // 通过评论ID获取评论
        Task DeleteCommentAsync(int id);               // 通过评论ID删除评论
    }
}

