using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController(ICommentService commentService) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;

        // 添加评论，必须登录
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { error = "用户身份无效" });

            if (comment == null || string.IsNullOrWhiteSpace(comment.Content))
                return BadRequest(new { error = "评论内容不能为空" });

            comment.UserId = int.Parse(userIdClaim.Value);
            comment.CreatedAt = DateTimeOffset.UtcNow;

            try
            {
                await _commentService.AddCommentAsync(comment);
                return Ok(new { message = "评论添加成功" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "服务器内部错误" });
            }
        }

        // 获取评论，公开接口
        [HttpGet("{id:int}")]
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

        // 删除评论，只有管理员或本人可删除
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { error = "用户身份无效" });

            int userId = int.Parse(userIdClaim.Value);

            try
            {
                var comment = await _commentService.GetCommentByIdAsync(id);
                if (comment == null)
                    return NotFound(new { error = "评论不存在" });

                var roleClaim = User.FindFirst(ClaimTypes.Role);
                bool isAdmin = roleClaim != null && roleClaim.Value == nameof(UserRole.ProjectAdmin);

                if (!isAdmin && comment.UserId != userId)
                    return Forbid("无权删除该评论");

                await _commentService.DeleteCommentAsync(id);
                return Ok(new { message = "评论删除成功" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "服务器内部错误" });
            }
        }
    }
}
