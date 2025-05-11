using Application.Interfaces;
using Domain.Entities;
using Domain.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        // 添加评论
        public async Task AddCommentAsync(Comment comment)
        {
            // 可以在这里进行一些验证，比如评论内容不能为空
            if (string.IsNullOrWhiteSpace(comment.Content))
            {
                throw new ArgumentException("评论内容不能为空");
            }

            await _commentRepository.AddAsync(comment);
        }

        // 根据任务ID获取评论列表
        public async Task<List<Comment>> GetCommentsByTaskIdAsync(int taskId)
        {
            // 可以根据任务ID查询评论
            return await _commentRepository.GetByTaskIdAsync(taskId);
        }

        // 根据用户ID获取评论列表
        public async Task<List<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            // 根据用户ID查询评论
            return await _commentRepository.GetByUserIdAsync(userId);
        }

        // 删除评论
        public async Task DeleteCommentAsync(int commentId)
        {
            // 获取评论
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                throw new ArgumentException("评论未找到");
            }

            // 删除评论
            await _commentRepository.DeleteByIdAsync(commentId);
        }
    }
}
