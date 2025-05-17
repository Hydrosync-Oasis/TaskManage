using Domain.Entities;

namespace Domain.Repository {
    public interface IProjectRepository {
        public Task<Project?> GetProjectByIdAsync(int id);

        public Task UpdateProjectInfo(Project project);
    }
}
