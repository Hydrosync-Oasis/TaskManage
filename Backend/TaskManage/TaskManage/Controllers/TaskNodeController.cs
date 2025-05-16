using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TaskManage.Controllers {
    [Route("[controller]/[action]")]
    public class TaskNodeController(ITaskService taskService) : Controller {
        [HttpPost]
        public async Task<ActionResult> InsertTask([FromBody] TaskDto? dto) {
            if (dto?.ProjectId is null || dto.Priority is null || dto.Deadline is null || dto.Title is null) {
                return BadRequest("参数不完整");
            }
            await taskService.AddTask(dto);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] TaskDto dto) {
            if (dto.ProjectId != null) {
                return BadRequest("不能设置/修改所属项目");
            }
            await taskService.UpdateTask(dto);
            return Ok();
        }
    }
}
