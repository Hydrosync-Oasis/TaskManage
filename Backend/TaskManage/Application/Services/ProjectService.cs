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
    }
}
