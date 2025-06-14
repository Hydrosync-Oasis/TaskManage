﻿using Domain.Entities;

namespace Application.Dtos
{
        // 注册请求 DTO
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    
    // 登录请求 DTO
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // 用户信息 DTO
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public required UserRole Role { get; set; }
    }

     // 登录结果 DTO（包含 token 和用户信息）
    public class LoginResultDto
    {
        public string Token { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }

    public class LoginRegisterDto {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
