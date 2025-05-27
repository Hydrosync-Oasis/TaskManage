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

        public async Task DeleteByCommentIdAsync(int id) {
            var task = await dbContext.TaskNodes.Include(x => x.Comments).FirstOrDefaultAsync(x=>x.Id == id);
            ArgumentNullException.ThrowIfNull(task);
            dbContext.Comments.Remove(task.Comments.First(x => x.Id == id));
        }

        public async Task AddAsync(Comment comment)
        {
            await dbContext.Comments.AddAsync(comment);
            await dbContext.SaveChangesAsync();  // 一定要调用这个，才能提交事务
        }

    }
}
