using Application.Interfaces;
using Application.Services;
using Domain.Repository;
using Infrastructure.Auth;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure.DependencyInjection {
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 注册服务
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IProjectService, ProjectService>();

            return services;
        }

        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            // 注册仓储
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            return services;
        }
        
    }
}
