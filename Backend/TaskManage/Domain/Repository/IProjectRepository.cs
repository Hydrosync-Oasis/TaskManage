using Domain.Entities;

namespace Domain.Repository {
    public interface IProjectRepository {
        public Task<Project?> GetProjectByIdAsync(int id);

        public Task UpdateProjectInfoAsync(Project project);

        public Task DeleteProjectAsync(int id);
    }
}
