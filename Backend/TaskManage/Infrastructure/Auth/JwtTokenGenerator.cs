using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Repository;
using Application.Interfaces;

namespace Infrastructure.Auth
{
    public class JwtTokenGenerator(IConfiguration configuration, IUserRepository userRepository) : IJwtTokenGenerator {
        public async Task<string> GenerateToken(int userId) {
            if ((await userRepository.GetUserByIdAsync(userId)) is null) {
                throw new ArgumentException("不存在该用户");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, (await userRepository.GetUserByIdAsync(userId))!.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,(await userRepository.GetUserRoleByIdAsync(userId)).ToString()!)
            };

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
