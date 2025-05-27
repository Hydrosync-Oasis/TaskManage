using System.Security.Claims;
using Application.Dtos;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
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
    public class TaskController(ITaskService taskService, IUserService userService) : ControllerBase {
        // 插入任务（管理员）
        [HttpPost("Add")]
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
        [HttpPut("Update")]
        public async Task<ActionResult> Update([FromBody] TaskDto dto)
        {
            try {
                if (dto.Id is null)
                    return BadRequest("必须指定task id");

                var info = await taskService.GetTaskInfo(dto.Id.Value);
                var uid = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (info.CreateUserId != uid)
                    return Forbid("你不是创建该任务的用户");

                if (dto.ProjectId != null && dto.ProjectId != info.ProjectId)
                    return BadRequest("不能设置/修改所属项目");

                await taskService.UpdateTask(dto);
                return Ok();
            } catch (Exception e) {
                return StatusCode(500, new { message = e.Message });
            }
        }
        // 添加评论（登录用户）
        [HttpPost("/api/Comment/Add")]
        public async Task<IActionResult> AddComment([FromBody] CommentDto dto)
        {
            // 验证用户身份
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { error = "用户身份无效" });

            // 验证请求体及评论内容
            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { error = "评论内容不能为空" });

            TaskNode taskNode;
            try
            {
                // 尝试获取任务节点，找不到则捕获异常返回404
                taskNode = await taskService.GetTaskNodeByIdAsync(dto.TaskId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "关联的任务不存在" });
            }

            int userId = int.Parse(userIdClaim.Value);

            // 验证用户信息
            var userDto = await userService.GetUserInfo(userId);
            if (userDto == null)
                return Unauthorized(new { error = "用户不存在" });

            User user;
            try
            {
                user = await userService.GetUserById(userId);
            }
            catch (KeyNotFoundException)
            {
                return Unauthorized(new { error = "用户不存在" });
            }

            // 创建评论实体
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
                // 捕获其他异常，返回500错误
                return StatusCode(500, new { error = e.Message });
            }
        }




        // 获取评论（公开）
        [HttpGet("/api/Comment/{id:int}")]
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
        [HttpDelete("/api/Comment/Delete/{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { error = "用户身份无效" });

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { error = "用户身份无效" });

            try
            {
                var comment = await taskService.GetCommentByIdAsync(id);
                if (comment == null)
                    return NotFound(new { error = "评论不存在" });

                var user = await userService.GetUserById(userId);

                // 判断角色权限
                bool isSystemAdmin = user.UserRole == UserRole.Admin;
                bool isProjectAdmin = user.UserRole == UserRole.ProjectAdmin;

                bool isOwner = comment.Owner.Id == userId;

                if (!isSystemAdmin && !isProjectAdmin && !isOwner)
                    return Forbid();

                await taskService.DeleteCommentAsync(id);

                return Ok(new { message = "评论删除成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }



        //通过任务ID获取所有评论(登录用户)
        [HttpGet("{taskId:int}/Comments")]
        public async Task<IActionResult> GetCommentsByTaskId(int taskId)
        {
            try
            {
                var comments = await taskService.GetAllCommentsByTaskIdAsync(taskId);
                if (comments.Count == 0)
                    return NotFound(new { message = "该任务下暂无评论" });

                // 映射 Comment 实体到 CommentDto，包含 CreatedTime
                var commentDtos = comments.Select(c => new CommentDto
                {
                    CommentId = c.Id,
                    TaskId = c.Task.Id,
                    UserId = c.Owner.Id,
                    Content = c.Content,
                    CreatedTime = c.CreatedTime  // 这里加上创建时间
                }).ToList();

                return Ok(commentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 获取任务信息（任意登录用户）
        [HttpGet("Info/{id:int}")]
        public async Task<IActionResult> GetTaskInfo(int id)
        {
            try {
                var taskDto = await taskService.GetTaskInfo(id);
                return Ok(taskDto);
            } catch (KeyNotFoundException ex) {
                return NotFound(new { error = ex.Message});
            } catch (Exception ex) {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
