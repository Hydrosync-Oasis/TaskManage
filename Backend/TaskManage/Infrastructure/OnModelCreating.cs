using Domain.Entities;
using Infrastructure.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;



namespace Infrastructure
{
    public class TaskManageDbContext(DbContextOptions<TaskManageDbContext> options) : DbContext(options) {
        public DbSet<AuditLogs> AuditLogs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TaskNode> TaskNodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);// 从指定的程序集自动找到所有的配置类
        }
    }

    /// <summary>
    /// 需要告诉EF core如何传入Options
    /// </summary>
    public class TaskManageDbContextFactory : IDesignTimeDbContextFactory<TaskManageDbContext> {
        public TaskManageDbContext CreateDbContext(string[] args) {
            // 引入appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../TaskManage");
            IConfiguration conf = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TaskManageDbContext>();
            optionsBuilder.UseMySql(
                conf.GetConnectionString("DefaultConnection")!, new MySqlServerVersion(new Version(8, 0)));

            return new TaskManageDbContext(optionsBuilder.Options);
        }
    }
}
