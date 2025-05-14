using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository {
    public class CommentRepository(TaskManageDbContext dbContext) : ICommentRepository {
        public Task<Comment?> GetByCommentIdAsync(int id) {
            return dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Comment>> GetByTaskIdAsync(int id) {
            return dbContext.Comments.Where(x => x.Task.Id == id).ToListAsync();
        }

        public Task<List<Comment>> GetByUserIdAsync(int id) {
            return dbContext.Comments.Where(x => x.Owner.Id == id).ToListAsync();
        }

        public async Task DeleteByCommentIdAsync(int id) {
            var comment = dbContext.Comments.FirstOrDefault(x => x.Id == id);
            if (comment != null) {
                dbContext.Comments.Remove(comment);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task AddAsync(Comment comment) {
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();
        }
    }
}
