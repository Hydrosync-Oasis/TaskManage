using System.Security.Cryptography.X509Certificates;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repository;
using Infrastructure;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProjectService(IProjectRepository projectRepository, ITaskRepository taskRepository)
        : IProjectService {
        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            var projects = await projectRepository.GetAllProjectsAsync();
            return projects.Adapt<IEnumerable<ProjectDto>>();
        }

        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var project = await projectRepository.GetProjectByIdAsync(id);
            return project?.Adapt<ProjectDto>();
        }

        public async Task<ProjectDto> CreateAsync(ProjectDto dto)
        {
            var entity = dto.Adapt<Project>();
            entity.OwnerId = dto.OwnerUid!.Value;
            var created = await projectRepository.CreateProjectAsync(entity);
            return created.Adapt<ProjectDto>();
        }

        public async Task<ProjectDto?> UpdateAsync(int id, ProjectDto dto)
        {
            var proj = await projectRepository.GetProjectByIdAsync(id);
            if (proj == null) return null;

            if (dto.Description is not null)
                proj.Description = dto.Description;

            if (dto.OwnerUid is not null && dto.OwnerUid != proj.OwnerId)
                throw new ArgumentException("不能更改所有者");

            if (dto.Name is not null)
                proj.Name = dto.Name;

            if (dto.CreatedAt is not null && dto.CreatedAt != proj.CreatedAt)
                throw new ArgumentException("不能更改创建时间");

            await projectRepository.UpdateProjectInfoAsync(proj);
            return proj.Adapt<ProjectDto>();
        }

        public Task DeleteAsync(int id)
        {
            return projectRepository.DeleteProjectAsync(id);
        }

        public async Task<List<TaskDto>> GetTasksByProjectIdAsync(int projectId)
        {
            var tasks = await taskRepository.GetTasksByProjectIdAsync(projectId);
            return tasks.Adapt<List<TaskDto>>();
        }

        public async Task<List<List<TaskDto>>> GetTopologicalOrder(int projectId) {
            var allTasks = await GetTasksByProjectIdAsync(projectId);
            if (allTasks is null) {
                throw new ArgumentOutOfRangeException(nameof(projectId), "不存在该id");
            }

            Dictionary<int, TaskDto> idMap = [];
            Queue<(TaskDto value, int level)> queue = [];
            Dictionary<TaskDto, int> indegreeDictionary = []; // dto.id对应入度
            List<List<TaskDto>> res = [[]];

            foreach (var tasks in allTasks) {
                idMap[tasks.Id!.Value] = tasks;
            }

            foreach (var task in allTasks) {
                indegreeDictionary.TryAdd(task, 0);
                task.DependencyTaskIds.ForEach((x) => indegreeDictionary[idMap[x]]++);
            }

            foreach (var task in indegreeDictionary) {
                if (task.Value == 0) {
                    queue.Enqueue((task.Key, 0));
                }
            }

            while (queue.Count > 0) {
                var task = queue.Dequeue();
                if (task.level == res.Count) {
                    res.Add([]);
                }
                res[task.level].Add(task.value);
                foreach (var key in task.value.DependencyTaskIds!) {
                    var nodeOfKey = idMap[key];
                    var newIn = --indegreeDictionary[nodeOfKey];
                    if (newIn == 0) {
                        queue.Enqueue((nodeOfKey, task.level + 1));
                    }
                }
            }

            res.Reverse();
            return res;
        }


    }
}
