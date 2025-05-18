using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManage.Controllers {
    [Authorize]
    [Route("[controller]/[action]")]
    public class TaskNodeController(ITaskService taskService) : Controller {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> InsertTask([FromBody] TaskDto? dto) {
            if (dto?.ProjectId is null || dto.Priority is null || dto.Deadline is null || dto.Title is null) {
                return BadRequest("参数不完整");
            }

            try {
                var resultId = await taskService.AddTask(dto, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                return Ok(new {
                    TaskId = resultId
                });
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] TaskDto dto) {
            if (dto.Id is null) {
                return BadRequest("必须指定task id");
            }

            var info = await taskService.GetTaskInfo(dto.Id.Value);

            var uid = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (info.CreateUserId != uid) {
                return Forbid("你不是创建该任务的用户");
            }
            if (dto.ProjectId != null) {
                return BadRequest("不能设置/修改所属项目");
            }
            await taskService.UpdateTask(dto);
            return Ok();
        }
    }
}
