using Domain.Entities;

namespace Domain.Repository {
    public interface IUserRepository {
        public Task<User?> GetUserByIdAsync(int id);
        public Task<List<User>> GetUserByStartUsernameAsync(string uname);
        public Task<User?> GetUserByUsernameAsync(string uname);
        /// <summary>
        /// 查询用户权限角色
        /// </summary>
        /// <param name="id">用户内部id</param>
        /// <returns>角色信息</returns>
        public Task<UserRole?> GetUserRoleByIdAsync(int id);

        public Task AddUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task DeleteUserAsync(int id);
    }
}
