using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ArgumentNullException.ThrowIfNull(proj);
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
    }
}
