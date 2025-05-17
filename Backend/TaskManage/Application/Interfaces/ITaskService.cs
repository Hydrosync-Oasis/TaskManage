using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces {
    public interface ITaskService {
        public Task UpdateTask(TaskDto dto);

        public Task AddTask(TaskDto dto);

        public Task RemoveTask(int taskId);

        public Task<TaskDto> GetTaskInfo(int taskId);
    }
}
