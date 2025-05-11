using Application.Interfaces;
using Domain.Entities;
using Domain.Repository;
using System;
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
            if (string.IsNullOrWhiteSpace(comment.Content))
            {
                throw new ArgumentException("评论内容不能为空");
            }

            await _commentRepository.AddAsync(comment);  // 调用 AddAsync 方法
        }

        // 根据评论ID获取评论
        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByCommentIdAsync(id);  // 调用 GetByCommentIdAsync 方法
            if (comment == null)
            {
                throw new ArgumentException("未找到该评论");
            }

            return comment;
        }

        // 根据评论ID删除评论
        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _commentRepository.GetByCommentIdAsync(id);  // 调用 GetByCommentIdAsync 方法
            if (comment == null)
            {
                throw new ArgumentException("评论未找到");
            }

            await _commentRepository.DeleteByCommentIdAsync(id);  // 调用 DeleteByCommentIdAsync 方法
        }
    }
}
