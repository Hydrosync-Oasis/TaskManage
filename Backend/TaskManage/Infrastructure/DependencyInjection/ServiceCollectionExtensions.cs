using Application.Interfaces;
using Application.Services;
using Domain.Repository;
using Infrastructure.EntityConfiguration;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
            // 注册服务
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskService, TaskService>();

            return services;
        }

        public static IServiceCollection AddRepositoryServices(this IServiceCollection services) {
            // 注册仓储
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();


            return services;
        }
    }
}
