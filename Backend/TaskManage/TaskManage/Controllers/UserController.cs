using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Auth;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;

namespace TaskManage.Controllers {
    [Route("[controller]/[action]")]
    public class UserController(IUserService userService, IJwtTokenGenerator jwtTokenGenerator) : Controller {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] LoginRegisterDto? dto) {
            if (dto is null) {
                return BadRequest("请求体不合法");
            }

            if (string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Username)) {
                return BadRequest("请输入用户名和密码");
            }

            await userService.Register(dto.Username, dto.Password);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRegisterDto? dto) {
            if (dto is null) {
                return BadRequest("请求体不合法");
            }

            try {
                var result = await userService.Login(dto.Username, dto.Password);
                return Ok(result.Token);
            } catch (Exception e) {
                return Unauthorized(e.Message);
            }
        }

        [HttpGet("User/{uid:int}")]
        public async Task<UserDto> QueryUser(int uid) {
            return await userService.GetUserInfo(uid);
        }
    }
}
