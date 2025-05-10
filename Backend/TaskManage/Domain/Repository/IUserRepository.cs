using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Repository {
    public interface IUserRepository {
        public Task<User> GetUserByIdAsync(int id);
        public Task<User> GetUserByUsernameAsync(int id);

        public Task AddUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task DeleteUserAsync(int id);

        /// <summary>
        /// 获取一个用户目前正在处理多少个任务。
        /// </summary>
        /// <param name="user">用户实例</param>
        /// <returns>任务数</returns>
        public Task<int> GetPendingTaskCountAsync(User user);
    }
}
