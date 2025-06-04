using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Repository;

namespace Infrastructure.Auth
{
    public class JwtTokenGenerator(IConfiguration configuration, IUserRepository userRepository) : IJwtTokenGenerator
    {
        public async Task<string> GenerateToken(int userId)
        {
            // 获取用户信息
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("不存在该用户");

            // 密钥和签名
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 构建 Claims，包括角色
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()) // 关键角色信息
            };

            // 构建 Token
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
