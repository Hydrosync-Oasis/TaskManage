using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Repository {
    public interface ICommentRepository {
        public Task<Comment?> GetByCommentIdAsync(int id);
        public Task<List<Comment>> GetByTaskIdAsync(int id);
        public Task<List<Comment>> GetByUserIdAsync(int id);

        public Task DeleteByCommentIdAsync(int id);
        public Task AddAsync(Comment comment);

    }
}
