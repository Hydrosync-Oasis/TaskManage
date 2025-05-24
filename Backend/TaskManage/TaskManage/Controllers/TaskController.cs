using System.Security.Claims;
using Application.Dtos;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController(ITaskService taskService) : ControllerBase {
        private readonly IUserRepository userRepository;
        // 插入任务（管理员）
        [HttpPost("insert")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> InsertTask([FromBody] TaskDto? dto)
        {
            if (dto?.ProjectId is null || dto.Priority is null || dto.Deadline is null || dto.Title is null)
                return BadRequest("参数不完整");

            try
            {
                var resultId = await taskService.AddTask(dto, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                return Ok(new { TaskId = resultId });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // 更新任务（创建人）
        [HttpPost("update")]
        public async Task<ActionResult> Update([FromBody] TaskDto dto)
        {
            try {
                if (dto.Id is null)
                    return BadRequest("必须指定task id");

                var info = await taskService.GetTaskInfo(dto.Id.Value);
                var uid = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (info.CreateUserId != uid)
                    return Forbid("你不是创建该任务的用户");

                if (dto.ProjectId != null)
                    return BadRequest("不能设置/修改所属项目");

                await taskService.UpdateTask(dto);
                return Ok();
            } catch (Exception e) {
                return StatusCode(500, e.Message);
            }
        }

        // 添加评论（登录用户）
        [HttpPost("comment/add")]
        public async Task<IActionResult> AddComment([FromBody] CommentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { error = "用户身份无效" });

            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { error = "评论内容不能为空" });

            var taskNode = await taskService.GetTaskNodeByIdAsync(dto.TaskId);
            if (taskNode == null)
                return BadRequest(new { error = "关联的任务不存在" });

            var userId = int.Parse(userIdClaim.Value);
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null) return Unauthorized(new { error = "用户不存在" });

            var comment = new Comment
            {
                Content = dto.Content,
                Task = taskNode,
                Owner = user,
                CreatedTime = DateTimeOffset.UtcNow
            };


            try
            {
                await taskService.AddCommentAsync(comment);
                return Ok(new { message = "评论添加成功" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message});
            }
        }

        // 获取评论（公开）
        [HttpGet("comment/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComment(int id)
        {
            try
            {
                var comment = await taskService.GetCommentByIdAsync(id);
                if (comment == null)
                    return NotFound(new { error = "评论不存在" });

                return Ok(comment);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message});
            }
        }

        // 删除评论（管理员或本人）
        [HttpDelete("comment/{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { error = "用户身份无效" });

            int userId = int.Parse(userIdClaim.Value);

            try
            {
                var comment = await taskService.GetCommentByIdAsync(id);
                if (comment == null)
                    return NotFound(new { error = "评论不存在" });

                var roleClaim = User.FindFirst(ClaimTypes.Role);
                bool isAdmin = roleClaim != null && roleClaim.Value == nameof(UserRole.ProjectAdmin);

                if (!isAdmin && comment.Owner.Id != userId)
                    return Forbid("无权删除该评论");

                await taskService.DeleteCommentAsync(id);
                return Ok(new { message = "评论删除成功" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message});
            }
        }
    }
}
