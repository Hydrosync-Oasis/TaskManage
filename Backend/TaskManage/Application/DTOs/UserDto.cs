using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class LoginResultDto
    {
        public string Token { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}
