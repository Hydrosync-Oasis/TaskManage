using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.DTOMappings;
using Infrastructure;
using Infrastructure.DependencyInjection;
using Mapster;


namespace TaskManage {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Swagger 配置
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 注册 CORS 策略
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()      // 允许所有来源（开发环境可以，生产建议指定）
                        .AllowAnyHeader()      // 允许所有请求头
                        .AllowAnyMethod();     // 允许所有方法（GET, POST, PUT 等）
                });
            });

            // 数据库配置
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<TaskManageDbContext>(options =>
                options.UseSqlServer(
                    connectionString!));


            // DTO
            TaskMapping.Register(TypeAdapterConfig.GlobalSettings);
            ProjectMapping.Register(TypeAdapterConfig.GlobalSettings);
            CommentMapping.Register(TypeAdapterConfig.GlobalSettings);

            // 自己封装的依赖注入方法
            builder.Services.AddInfrastructureServices()
                .AddApplicationServices()
                .AddRepositoryServices();

            // JWT 身份验证服务注册
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                        )
                    };
                });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");  

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
