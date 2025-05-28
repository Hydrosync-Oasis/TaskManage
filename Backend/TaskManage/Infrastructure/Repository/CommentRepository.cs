using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository {
    public class CommentRepository(TaskManageDbContext dbContext) : ICommentRepository {
        public async Task<List<Comment>> GetAllCommentsByTaskIdAsync(int id) {
            var task = await dbContext.TaskNodes
                .Include(x => x.Comments)
                .Include(x => x.AssignedUser).FirstOrDefaultAsync(x => x.Id == id);
            return task == null ? throw new KeyNotFoundException("找不到该task") : task.Comments;
        }

        public async Task DeleteByCommentIdAsync(int id)
        {
            var comment = await dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
                throw new KeyNotFoundException($"找不到ID为 {id} 的评论");

            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();
        }


        public async Task AddAsync(Comment comment)
        {
            await dbContext.Comments.AddAsync(comment);
            await dbContext.SaveChangesAsync();  // 一定要调用这个，才能提交事务
        }

    }
}
