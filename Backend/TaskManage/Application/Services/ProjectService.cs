using Application.DTOs;
using Application.Interfaces;
using Domain.Repository;
using Mapster;

namespace Application.Services {
    public class ProjectService(IProjectRepository projectRepository) : IProjectService {
        public async Task<ProjectDto> GetProjectInfo(int projectId) {
            return (await projectRepository.GetProjectByIdAsync(projectId)).Adapt<ProjectDto>();
        }

        public async Task UpdateProjectInfo(ProjectDto dto) {
            var proj = await projectRepository.GetProjectByIdAsync(dto.Id);
            ArgumentNullException.ThrowIfNull(proj);

            if (dto.Description is not null) {
                proj.Description = dto.Description;
            }

            if (dto.OwnerUid is not null) {
                throw new ArgumentException("不能更改所有者");
            }

            if (dto.Name is not null) {
                proj.Name = dto.Name;
            }

            if (dto.CreatedAt is not null) {
                throw new ArgumentException("不能更改创建时间");
            }

            await projectRepository.UpdateProjectInfoAsync(proj);
        }

        public Task DeleteProject(int projectId) {
            return projectRepository.DeleteProjectAsync(projectId);
        }
    }
}
