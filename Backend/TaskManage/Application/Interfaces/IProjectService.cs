using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces {
    interface IProjectService {
        public Task<ProjectDto> GetProjectInfo(int projectId);

        public Task UpdateProjectInfo(ProjectDto dto);

        public Task DeleteProject(int projectId);
    }
}
