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
            // ��ȡ�û���Ϣ
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("�����ڸ��û�");

            // ��Կ��ǩ��
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // ���� Claims��������ɫ
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()) // �ؼ���ɫ��Ϣ
            };

            // ���� Token
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
