using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // 用户注册
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                await _userService.Register(dto.Username, dto.Password);
                return Ok(new { message = "注册成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 用户登录
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _userService.Login(dto.Username, dto.Password);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 受 JWT 保护的接口，只有登录后才可访问
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst(ClaimTypes.Name)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;

                return Ok(new
                {
                    userId,
                    username
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "获取用户信息失败: " + ex.Message });
            }
        }
    }
}
