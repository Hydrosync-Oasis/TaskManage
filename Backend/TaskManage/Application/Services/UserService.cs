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

        public async Task<UserDto> GetUserInfo(int id) {
            var u = await userRepository.GetUserByIdAsync(id);
            return new UserDto() {
                Username = u.UserName,
                Id = id,
                AvatarUrl = u.AvatarUrl,
                Role = u.UserRole
            };
        }
    }
}
