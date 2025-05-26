using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 登录用户才能访问所有接口
    public class ProjectController(IProjectService projectService) : ControllerBase {
        [HttpGet("AllProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await projectService.GetAllAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取项目列表失败", error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            try
            {
                var project = await projectService.GetByIdAsync(id);
                if (project == null)
                    return NotFound(new { message = "未找到该项目" });

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取项目详情失败", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto dto)
        {
            try
            {
                // 从当前登录用户的 Claims 中获取用户 ID
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "无法识别用户身份" });
                }

                if (!int.TryParse(userIdClaim.Value, out var userId))
                {
                    return BadRequest(new { message = "无效的用户ID" });
                }

                // 将用户 ID 设置到 dto.OwnerUid 中
                dto.OwnerUid = userId;

                var created = await projectService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetProjectById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "创建项目失败", error = ex.Message });
            }
        }

        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto dto)
        {
            try
            {
                 // 从 JWT 中获取用户 ID
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "无法识别的用户身份" });
                }

                dto.OwnerUid = int.Parse(userIdClaim.Value); 

                var updated = await projectService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = "未找到要更新的项目" });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新项目失败", error = ex.Message });
            }
        }

        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try {
                await projectService.DeleteAsync(id);

                return NoContent();
            } catch (ArgumentOutOfRangeException ex) {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "删除项目失败", error = ex.Message });
            }
        }

        // 新增接口：根据项目id获取任务列表
        [HttpGet("{id:int}/Tasks")]
        public async Task<ActionResult<List<TaskNode>>> GetTasksForProject(int id)
        {
            try
            {
                var tasks = await projectService.GetTasksByProjectIdAsync(id);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取任务列表失败", error = ex.Message });
            }
        }
    }
}
