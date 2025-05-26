using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Auth;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManage.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController(IUserService userService, IJwtTokenGenerator jwtTokenGenerator) : Controller
    {

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] LoginRegisterDto? dto)
        {
            if (dto is null)
            {
                return BadRequest("请求体不合法");
            }

            if (string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Username))
            {
                return BadRequest("请输入用户名和密码");
            }

            await userService.Register(dto.Username, dto.Password);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRegisterDto? dto)
        {
            if (dto is null)
            {
                return BadRequest("请求体不合法");
            }

            try
            {
                var result = await userService.Login(dto.Username, dto.Password);
                return Ok(result.Token);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpGet("User/{uid:int}")]
        public async Task<IActionResult> QueryUser(int uid) {
            var userDto = await userService.GetUserInfo(uid);
            if (userDto is null) {
                return NotFound(new {
                    Error = $"找不到uid={uid}的用户"
                });
            }

            return Ok(userDto);
        }

        /// <summary>
        /// 验证用户信息是否有效
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult Auth() {
            return Ok(new {
                message = "用户已登录"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto dto)
        {
            try
            {
                var user = await userService.CreateUserAsync(dto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> SetStatus(int userId, [FromQuery] bool isActive)
        {
            var success = await userService.SetUserActiveStatusAsync(userId, isActive);
            if (!success)
                return NotFound(new { message = "用户不存在" });

            return Ok(new { message = $"用户状态已更新为 {(isActive ? "激活" : "停用")}" });
        }
    }
}
