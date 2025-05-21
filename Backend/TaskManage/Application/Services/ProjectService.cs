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

        // 构造函数注入两个依赖
        public ProjectService(IProjectRepository projectRepository, TaskManageDbContext context)
        {
            _projectRepository = projectRepository;
            _context = context;
        }

        public async Task<ProjectDto> GetProjectInfo(int projectId)
        {
            var project = await _projectRepository.GetProjectByIdAsync(projectId);
            return project.Adapt<ProjectDto>();
        }

        public async Task UpdateProjectInfo(ProjectDto dto)
        {
            var proj = await _projectRepository.GetProjectByIdAsync(dto.Id);
            ArgumentNullException.ThrowIfNull(proj);

            if (dto.Description is not null)
            {
                proj.Description = dto.Description;
            }

            if (dto.OwnerUid is not null)
            {
                throw new ArgumentException("不能更改所有者");
            }

            if (dto.Name is not null)
            {
                proj.Name = dto.Name;
            }

            if (dto.CreatedAt is not null)
            {
                throw new ArgumentException("不能更改创建时间");
            }

            await _projectRepository.UpdateProjectInfoAsync(proj);
        }

        public Task DeleteProject(int projectId)
        {
            return _projectRepository.DeleteProjectAsync(projectId);
        }

        public async Task<List<TaskNode>> GetTasksByProjectIdAsync(int projectId)
        {
            return await _context.TaskNodes
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }
    }
}
