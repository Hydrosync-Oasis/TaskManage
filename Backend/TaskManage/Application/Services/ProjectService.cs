using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repository;
using Infrastructure;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly TaskManageDbContext _context;

        public ProjectService(IProjectRepository projectRepository, TaskManageDbContext context)
        {
            _projectRepository = projectRepository;
            _context = context;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            var projects = await _projectRepository.GetAllProjectsAsync();
            return projects.Adapt<IEnumerable<ProjectDto>>();
        }

        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);
            return project?.Adapt<ProjectDto>();
        }

        public async Task<ProjectDto> CreateAsync(ProjectDto dto)
        {
            var entity = dto.Adapt<Project>();
            var created = await _projectRepository.CreateProjectAsync(entity);
            return created.Adapt<ProjectDto>();
        }

        public async Task<ProjectDto?> UpdateAsync(int id, ProjectDto dto)
        {
            var proj = await _projectRepository.GetProjectByIdAsync(id);
            if (proj == null) return null;

            if (dto.Description is not null)
                proj.Description = dto.Description;

            if (dto.OwnerUid is not null)
                throw new ArgumentException("不能更改所有者");

            if (dto.Name is not null)
                proj.Name = dto.Name;

            if (dto.CreatedAt is not null)
                throw new ArgumentException("不能更改创建时间");

            await _projectRepository.UpdateProjectInfoAsync(proj);
            return proj.Adapt<ProjectDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id);
            if (project == null) return false;

            await _projectRepository.DeleteProjectAsync(id);
            return true;
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId)
        {
            var tasks = await _context.TaskNodes
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();

            return tasks.Adapt<IEnumerable<TaskDto>>();
        }

        public async Task<TaskDto> AddTaskToProjectAsync(int projectId, TaskDto taskDto)
        {
            var task = taskDto.Adapt<TaskNode>();
            task.ProjectId = projectId;
            _context.TaskNodes.Add(task);
            await _context.SaveChangesAsync();
            return task.Adapt<TaskDto>();
        }

        public async Task<bool> RemoveTaskFromProjectAsync(int projectId, int taskId)
        {
            var task = await _context.TaskNodes
                .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId);

            if (task == null) return false;

            _context.TaskNodes.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
