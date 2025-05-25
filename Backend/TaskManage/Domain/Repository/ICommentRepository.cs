using Domain.Entities;

namespace Domain.Repository {
    public interface ICommentRepository {
        public Task<List<Comment>> GetAllCommentsByTaskIdAsync(int id);

        public Task DeleteByCommentIdAsync(int id);
        public Task AddAsync(Comment comment);

    }
}
