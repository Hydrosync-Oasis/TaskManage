using Domain.Entities;

namespace Domain.Repository {
    public interface IProjectRepository {
        public Task<Project?> GetProjectByIdAsync(int id);
        /// <summary>
        /// 判断该项目是否出现了环形依赖
        /// </summary>
        /// <returns>true代表出现了环形依赖需要修复，否则代表没有问题。</returns>
        public Task<bool> HasCycleAsync();
    }
}
