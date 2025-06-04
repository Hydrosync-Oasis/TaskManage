using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository {
    public class UserRepository(TaskManageDbContext dbContext) :IUserRepository {
        public Task<User?> GetUserByIdAsync(int id) {
            return dbContext.Users.FirstOrDefaultAsync(x=> x.Id == id);
        }

        public async Task<List<User>> GetUserByStartUsernameAsync(string uname) {
            return await dbContext.Users.Where(x => x.UserName.StartsWith(uname)).ToListAsync();
        }

        public async Task<UserRole?> GetUserRoleByIdAsync(int id) {
            return (await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id))?.UserRole;
        }

        public async Task AddUserAsync(User user) {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user) {
            dbContext.Update(user);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id) {
            var res = await dbContext.Users.FirstAsync(x => x.Id == id);
            dbContext.Remove(res);
            await dbContext.SaveChangesAsync();
        }

        public Task<User?> GetUserByUsernameAsync(string uname) {
            return dbContext.Users.FirstOrDefaultAsync(x => x.UserName == uname);
        }
    }
}
