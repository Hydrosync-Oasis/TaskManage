using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ProjectRepository(TaskManageDbContext dbContext) : IProjectRepository
    {
        public Task<Project?> GetProjectByIdAsync(int id)
        {
            return dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateProjectInfoAsync(Project project)
        {
            dbContext.Projects.Update(project);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var proj = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if (proj is null) {
                throw new ArgumentOutOfRangeException(nameof(id), "没有该id的项目");
            }
            dbContext.Projects.Remove(proj);
            await dbContext.SaveChangesAsync(); // ✅ 添加保存
        }

        // 实现接口方法 GetAllProjectsAsync
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await dbContext.Projects.ToListAsync();
        }

        // 实现接口方法 CreateProjectAsync
        public async Task<Project> CreateProjectAsync(Project project)
        {
            dbContext.Projects.Add(project);
            await dbContext.SaveChangesAsync();
            return project;
        }

        public async Task<bool> IsProjectExists(int projectId) {
            return await dbContext.Projects.AnyAsync(x => x.Id == projectId);
        }

    }
}
