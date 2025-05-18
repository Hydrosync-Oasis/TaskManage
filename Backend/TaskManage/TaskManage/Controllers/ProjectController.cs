using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;

namespace TaskManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取项目列表失败", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            try
            {
                var project = await _projectService.GetByIdAsync(id);
                if (project == null)
                    return NotFound(new { message = "未找到该项目" });

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取项目详情失败", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto dto)
        {
            try
            {
                var created = await _projectService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetProjectById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "创建项目失败", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto dto)
        {
            try
            {
                var updated = await _projectService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = "未找到要更新的项目" });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新项目失败", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var deleted = await _projectService.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "未找到要删除的项目" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "删除项目失败", error = ex.Message });
            }
        }
    }
}
