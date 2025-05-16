using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository {
    public class ProjectRepository(TaskManageDbContext dbContext) : IProjectRepository {
        public Task<Project?> GetProjectByIdAsync(int id) {
            return dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateProjectInfo(Project project) {
            dbContext.Projects.Update(project);
            await dbContext.SaveChangesAsync();
        }
    }
}
