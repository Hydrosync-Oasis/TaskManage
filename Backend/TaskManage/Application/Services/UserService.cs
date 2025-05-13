using Application.DTOs;
using Application.Interfaces;
using BCrypt.Net;
using Domain.Entities;
using Domain.Repository;
using Infrastructure.Auth; // 引入 JwtTokenGenerator 所在命名空间
namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public UserService(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task Register(string username, string password)
        {
            // 检查用户名是否存在
            var res = await _userRepository.GetUserByUsernameAsync(username);
            var exists = res is not null;
            if (exists)
                throw new InvalidOperationException("用户名已存在");

            // 加密密码
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                UserName = username,
                PasswordHash = hashedPassword,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _userRepository.AddUserAsync(user);
        }

        public async Task<LoginResultDto> Login(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
                throw new UnauthorizedAccessException("用户名或密码错误");

            bool passwordOk = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordOk)
                throw new UnauthorizedAccessException("用户名或密码错误");

            // 自定义生成JWT
            string token = _jwtTokenGenerator.GenerateToken(user.Id.ToString(), user.UserName);

            return new LoginResultDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    AvatarUrl = user.AvatarUrl
                }
            };
        }
    }
