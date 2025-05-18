using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces {
    public interface ITaskNodeService {
        public Task UpdateTask(TaskDto dto);

        /// <summary>
        /// 创建一个任务
        /// </summary>
        /// <param name="dto">任务描述对象</param>
        /// <param name="uid">创建者id</param>
        /// <returns></returns>
        public Task<int> AddTask(TaskDto dto, int uid);

        public Task RemoveTask(int taskId);

        public Task<TaskDto> GetTaskInfo(int taskId);
    }
}
