﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IProjectService
    {
        // 获取所有项目
        Task<IEnumerable<ProjectDto>> GetAllAsync();

        // 根据ID获取项目
        Task<ProjectDto?> GetByIdAsync(int id);

        // 创建项目
        Task<ProjectDto> CreateAsync(ProjectDto dto);

        // 更新项目
        Task<ProjectDto?> UpdateAsync(int id, ProjectDto dto);

        // 删除项目（返回是否成功）
        Task<bool> DeleteAsync(int id);
        //获取某个项目下的所有任务
        Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId);

        //为某个项目添加任务
        Task<TaskDto> AddTaskToProjectAsync(int projectId, TaskDto taskDto);

        //从项目中移除一个任务
        Task<bool> RemoveTaskFromProjectAsync(int projectId, int taskId);
    }
}