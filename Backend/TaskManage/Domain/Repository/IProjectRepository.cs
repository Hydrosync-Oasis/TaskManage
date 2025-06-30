using Domain.Entities;

namespace Domain.Repository {
    public interface IProjectRepository {
        public Task<IEnumerable<Project>> GetAllProjectsAsync();
        public Task<Project> CreateProjectAsync(Project project);
        public Task<Project?> GetProjectByIdAsync(int id);

        public Task UpdateProjectInfoAsync(Project project);

        public Task DeleteProjectAsync(int id);

        public Task<bool> IsProjectExists(int projectId);
    }
}
