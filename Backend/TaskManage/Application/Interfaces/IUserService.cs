using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task Register(string username, string password);

        Task<LoginResultDto> Login(string username, string password);

        Task<UserDto> GetUserById(int id);

        Task<UserDto> CreateUserAsync(UserDto dto);

        Task<bool> SetUserActiveStatusAsync(int userId, bool isActive);

        Task<bool> IsUserExists(int id);
    }
}
