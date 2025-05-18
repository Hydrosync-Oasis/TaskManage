using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // 添加评论，必须登录
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            try
            {
                // 从JWT token中获取用户ID，设置到评论里
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { error = "用户身份无效" });

                comment.UserId = int.Parse(userIdClaim.Value);

                await _commentService.AddCommentAsync(comment);
                return Ok(new { message = "评论添加成功" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "服务器内部错误" });
            }
        }

        // 获取评论，公开接口，无需登录
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComment(int id)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(id);
                if (comment == null)
                    return NotFound(new { error = "评论不存在" });
                
                return Ok(comment);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "服务器内部错误" });
            }
        }

        // 删除评论，需要登录，且只有评论所有者或管理员可删除
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { error = "用户身份无效" });

                int userId = int.Parse(userIdClaim.Value);

                var comment = await _commentService.GetCommentByIdAsync(id);
                if (comment == null)
                    return NotFound(new { error = "评论不存在" });

                // 判断是否是管理员或评论所有者
                // 这里要根据JWT里角色的Claim来判断，假设你在Token中角色Claim类型是 ClaimTypes.Role
                var roleClaim = User.FindFirst(ClaimTypes.Role);
                bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

                if (!isAdmin && comment.UserId != userId)
                    return Forbid("无权删除该评论");

                await _commentService.DeleteCommentAsync(id);
                return Ok(new { message = "评论删除成功" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "服务器内部错误" });
            }
        }
    }
}
