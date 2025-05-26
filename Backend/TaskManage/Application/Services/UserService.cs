using Application.DTOs;
using Application.Interfaces;
using BCrypt.Net;
using Domain.Entities;
using Domain.Repository;
using Infrastructure.Auth;

namespace Application.Services {
    public class UserService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        : IUserService {
        public async Task Register(string username, string password) {
            // 检查用户名是否存在
            var res = await userRepository.GetUserByUsernameAsync(username);
            var exists = res is not null;
            if (exists)
                throw new InvalidOperationException("用户名已存在");

            // 加密密码
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User {
                UserName = username,
                PasswordHash = hashedPassword,
                CreatedAt = DateTimeOffset.UtcNow,
                UserRole = UserRole.ProjectUser
            };

            await userRepository.AddUserAsync(user);
        }

        public async Task<LoginResultDto> Login(string username, string password) {
            var user = await userRepository.GetUserByUsernameAsync(username);
            if (user == null)
                throw new UnauthorizedAccessException("用户名或密码错误");

            bool passwordOk = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordOk)
                throw new UnauthorizedAccessException("用户名或密码错误");

            // 自定义生成JWT
            string token = await jwtTokenGenerator.GenerateToken(user.Id);

            return new LoginResultDto {
                Token = token,
                User = new UserDto {
                    Id = user.Id,
                    Username = user.UserName,
                    AvatarUrl = user.AvatarUrl,
                    Role = user.UserRole
                }
            };
        }

        public async Task<UserDto?> GetUserInfo(int id) {
            var u = await userRepository.GetUserByIdAsync(id);
            if (u is null) {
                return null;
            }
            return new UserDto() {
                Username = u.UserName,
                Id = id,
                AvatarUrl = u.AvatarUrl,
                Role = u.UserRole
            };
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"未找到ID为 {id} 的用户");
            return user;
        }
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var existing = await userRepository.GetUserByUsernameAsync(userDto.Username);
            if (existing != null)
                throw new InvalidOperationException("用户名已存在");

            var user = new User
            {
                UserName = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password), // 确保 UserDto 中有 Password 字段
                CreatedAt = DateTimeOffset.UtcNow,
                UserRole = userDto.Role,
                IsActive = true
            };

            await userRepository.AddUserAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                AvatarUrl = user.AvatarUrl,
                Role = user.UserRole
            };
        }

        // 管理员设置用户激活/禁用状态
        public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"未找到ID为 {userId} 的用户");

            user.IsActive = isActive;
            await userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}
