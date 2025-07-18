﻿using System.Security.Claims;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions.Dto;
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
                return StatusCode(500, new { error = e.Message });
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
            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { error = "评论内容不能为空" });

            if (!await taskService.IsTaskExists(dto.TaskId)) {
                return NotFound(new {
                    error = $"不存在id为{dto.TaskId}的任务"
                });
            }

            int userId = int.Parse(userIdClaim.Value);

            if (!await userService.IsUserExists(userId)) {
                return Unauthorized(new { error = "用户不存在" });
            }


            try {
                await taskService.AddCommentAsync(dto);
                return Ok(new { message = "评论添加成功" });
            } catch (DtoFieldOutOfRangeException e) {
                return NotFound(e.Message);
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
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { error = "用户身份无效" });

            try
            {
                // 从服务层获取评论，可能为空
                var comment = await taskService.GetCommentByIdAsync(id);
                if (comment == null)
                    return NotFound(new { error = $"找不到ID为 {id} 的评论" });

                var user = await userService.GetUserById(userId);

                // 判断角色权限
                bool isSystemAdmin = user.Role == UserRole.Admin;
                bool isProjectAdmin = user.Role == UserRole.ProjectAdmin;
                bool isOwner = comment.UserId == userId;

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
                var commentDtos = await taskService.GetAllCommentsByTaskIdAsync(taskId);
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

        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { error = "用户身份无效" });

            try
            {
                var exists = await taskService.IsTaskExists(id);
                if (!exists) {
                    return NotFound(new {
                        error = $"id为{id}的任务不存在"
                    });
                }

                var user = await userService.GetUserById(userId);

                bool isSystemAdmin = user.Role == UserRole.Admin;
                bool isProjectAdmin = user.Role == UserRole.ProjectAdmin;

                if (!isSystemAdmin && !isProjectAdmin)
                    return Forbid();

                await taskService.RemoveTask(id);

                return Ok(new { message = "任务节点删除成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
