using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        // 用户注册
        Task Register(string username, string password);

        // 用户登录
        Task<LoginResultDto> Login(string username, string password);

        // 获取用户信息（用于展示）
        Task<UserDto> GetUserInfo(int id);

        // 获取完整 User 实体（内部使用）
        Task<User> GetUserById(int id);

        // 超级管理员添加用户（带角色、激活状态）
        Task<UserDto> CreateUserAsync(UserDto dto);

        // 设置用户状态（激活/停用）
        Task<bool> SetUserActiveStatusAsync(int userId, bool isActive);
    }
}
