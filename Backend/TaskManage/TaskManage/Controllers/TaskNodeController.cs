using System.Security.Claims;
using Application.Interfaces;
using Application.Dtos;  // 引用你的 DTO 命名空间
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ITaskNodeService _taskNodeService;

        public CommentController(ICommentService commentService, ITaskNodeService taskNodeService)
        {
            _commentService = commentService;
            _taskNodeService = taskNodeService;
        }

        // 添加评论，要求登录
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { error = "用户身份无效" });

            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { error = "评论内容不能为空" });

            // 验证 TaskNode 是否存在
            var taskNode = await _taskNodeService.GetTaskNodeByIdAsync(dto.TaskId);
            if (taskNode == null)
                return BadRequest(new { error = "关联的任务不存在" });

            var comment = new Comment
            {
                Content = dto.Content,
                Task = taskNode,
                Owner = new User { Id = int.Parse(userIdClaim.Value) },
                CreatedTime = DateTimeOffset.UtcNow
            };

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

        // 删除评论，管理员或本人可操作
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

                if (!isAdmin && comment.Owner.Id != userId)
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
