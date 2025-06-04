using Application.Dtos;

namespace Application.Interfaces
{
    public interface IProjectService
    {
        /// <summary>
        /// 获取所有项目
        /// </summary>
        /// <returns>项目Dto描述</returns>
        Task<IEnumerable<ProjectDto>> GetAllAsync();

        // 根据ID获取项目
        Task<ProjectDto?> GetByIdAsync(int id);

        // 创建项目
        Task<ProjectDto> CreateAsync(ProjectDto dto);

        // 更新项目
        Task<ProjectDto?> UpdateAsync(int id, ProjectDto dto);

        // 删除项目（返回是否成功）
        Task DeleteAsync(int id);
        //获取某个项目下的所有任务
        Task<List<TaskDto>> GetTasksByProjectIdAsync(int projectId);

        /// <summary>
        /// 获取项目下所有任务的拓扑排序结果
        /// </summary>
        /// <param name="projectId">项目id</param>
        /// <returns></returns>
        Task<List<List<TaskDto>>> GetTopologicalOrder(int projectId);
    }
}