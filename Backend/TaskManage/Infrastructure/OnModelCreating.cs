using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Entities {
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
}
